using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PokheraliDevelopers.Data;
using PokheraliDevelopers.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PokheraliDevelopers.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CartController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: api/Cart
        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var cartItems = await _context.CartItems
                .Where(ci => ci.UserId == userId)
                .Include(ci => ci.Book)
                .ToListAsync();

            var cartDto = new
            {
                items = cartItems.Select(ci => new
                {
                    id = ci.Id,
                    bookId = ci.BookId,
                    bookTitle = ci.Book.Title,
                    bookAuthor = ci.Book.Author,
                    bookImageUrl = ci.Book.ImageUrl,
                    bookPrice = ci.Book.Price,
                    quantity = ci.Quantity,
                    subtotal = ci.Book.Price * ci.Quantity
                }).ToList(),
                totalItems = cartItems.Sum(ci => ci.Quantity),
                totalPrice = cartItems.Sum(ci => ci.Book.Price * ci.Quantity)
            };

            return Ok(cartDto);
        }

        // POST: api/Cart/add
        [HttpPost("add")]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartDto addToCartDto)
        {
            if (!ModelState.IsValid)
            {
                
                return BadRequest(ModelState);
            }

            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            // Check if the book exists
            var book = await _context.Books.FindAsync(addToCartDto.BookId);
            if (book == null)
            {
                return NotFound("Book not found");
            }

            // Check if the book is already in the cart
            var existingCartItem = await _context.CartItems
                .FirstOrDefaultAsync(ci => ci.UserId == userId && ci.BookId == addToCartDto.BookId);

            if (existingCartItem != null)
            {
                // Update the quantity
                existingCartItem.Quantity += addToCartDto.Quantity;

                // Check if quantity exceeds stock
                if (book.Stock > 0 && existingCartItem.Quantity > book.Stock)
                {
                    existingCartItem.Quantity = book.Stock;
                }

                _context.CartItems.Update(existingCartItem);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    id = existingCartItem.Id,
                    bookId = existingCartItem.BookId,
                    bookTitle = book.Title,
                    bookAuthor = book.Author,
                    bookImageUrl = book.ImageUrl,
                    bookPrice = book.Price,
                    quantity = existingCartItem.Quantity,
                    subtotal = book.Price * existingCartItem.Quantity
                });
            }
            else
            {
                // Create a new cart item
                var quantity = addToCartDto.Quantity;

                // Check if quantity exceeds stock
                if (book.Stock > 0 && quantity > book.Stock)
                {
                    quantity = book.Stock;
                }

                var newCartItem = new CartItem
                {
                    UserId = userId,
                    BookId = addToCartDto.BookId,
                    Quantity = quantity,
                    DateAdded = DateTime.UtcNow
                };

                _context.CartItems.Add(newCartItem);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    id = newCartItem.Id,
                    bookId = newCartItem.BookId,
                    bookTitle = book.Title,
                    bookAuthor = book.Author,
                    bookImageUrl = book.ImageUrl,
                    bookPrice = book.Price,
                    quantity = newCartItem.Quantity,
                    subtotal = book.Price * newCartItem.Quantity
                });
            }
        }

        // PUT: api/Cart/update
        [HttpPut("update")]
        public async Task<IActionResult> UpdateCartItem([FromBody] UpdateCartItemDto updateCartItemDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var cartItem = await _context.CartItems
                .Include(ci => ci.Book)
                .FirstOrDefaultAsync(ci => ci.Id == updateCartItemDto.CartItemId && ci.UserId == userId);

            if (cartItem == null)
            {
                return NotFound("Cart item not found");
            }

            // Check if the requested quantity is valid
            if (updateCartItemDto.Quantity <= 0)
            {
                // Remove the item if quantity is 0 or negative
                _context.CartItems.Remove(cartItem);
            }
            else
            {
                // Update the quantity
                if (cartItem.Book.Stock > 0 && updateCartItemDto.Quantity > cartItem.Book.Stock)
                {
                    cartItem.Quantity = cartItem.Book.Stock;
                }
                else
                {
                    cartItem.Quantity = updateCartItemDto.Quantity;
                }

                _context.CartItems.Update(cartItem);
            }

            await _context.SaveChangesAsync();
            return Ok();
        }

        // DELETE: api/Cart/{cartItemId}
        [HttpDelete("{cartItemId}")]
        public async Task<IActionResult> RemoveFromCart(int cartItemId)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(ci => ci.Id == cartItemId && ci.UserId == userId);

            if (cartItem == null)
            {
                return NotFound("Cart item not found");
            }

            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();
            return Ok();
        }

        // DELETE: api/Cart/clear
        [HttpDelete("clear")]
        public async Task<IActionResult> ClearCart()
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User is not authenticated.");
            }

            var cartItems = await _context.CartItems
                .Where(ci => ci.UserId == userId)
                .ToListAsync();

            _context.CartItems.RemoveRange(cartItems);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }

    public class AddToCartDto
    {
        public int BookId { get; set; }
        public int Quantity { get; set; } = 1;
    }

    public class UpdateCartItemDto
    {
        public int CartItemId { get; set; }
        public int Quantity { get; set; }
    }
}
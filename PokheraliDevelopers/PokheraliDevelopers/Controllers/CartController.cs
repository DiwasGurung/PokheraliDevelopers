// Controllers/CartController.cs (updated)
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class CartController : ControllerBase
{
    private readonly DatabaseHandlerEfCoreExample _context;

    public CartController(DatabaseHandlerEfCoreExample context)
    {
        _context = context;
    }

    // GET: api/Cart
    [HttpGet]
    public async Task<ActionResult<CartDto>> GetCart()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var cartItems = await _context.CartItems
            .Where(c => c.UserId == userId)
            .Include(c => c.Book)
            .ToListAsync();

        // Check if user has a profile, if not create one
        var userProfile = await _context.UserProfiles.FindAsync(userId);
        if (userProfile == null)
        {
            userProfile = new UserProfile
            {
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };
            _context.UserProfiles.Add(userProfile);
            await _context.SaveChangesAsync();
        }

        var cartDto = new CartDto
        {
            Items = cartItems.Select(c => new CartItemDto
            {
                Id = c.Id,
                BookId = c.BookId,
                BookTitle = c.Book.Title,
                BookAuthor = c.Book.Author,
                BookImageUrl = c.Book.ImageUrl,
                Quantity = c.Quantity,
                UnitPrice = c.Book.Price,
                DiscountPercentage = c.Book.IsOnSale &&
                                   c.Book.DiscountPercentage.HasValue &&
                                   c.Book.DiscountStartDate <= DateTime.UtcNow &&
                                   (!c.Book.DiscountEndDate.HasValue || c.Book.DiscountEndDate >= DateTime.UtcNow)
                                   ? c.Book.DiscountPercentage
                                   : null,
                LineTotal = CalculateLineTotal(c.Book, c.Quantity)
            }).ToList()
        };

        // Calculate totals and check for discounts
        cartDto.Subtotal = cartDto.Items.Sum(i => i.LineTotal);

        // Check if order qualifies for bulk discount (5+ books)
        cartDto.QualifiesForBulkDiscount = cartItems.Sum(c => c.Quantity) >= 5;

        // Check if user has stackable discount
        cartDto.HasStackableDiscount = userProfile.SuccessfulOrdersCount > 0
                                    && userProfile.SuccessfulOrdersCount % 10 == 0
                                    && !userProfile.StackableDiscountUsed;

        // Calculate discount amount
        decimal discountRate = 0;
        if (cartDto.QualifiesForBulkDiscount)
        {
            discountRate += 0.05m; // 5% bulk discount
        }

        if (cartDto.HasStackableDiscount)
        {
            discountRate += 0.10m; // 10% stackable discount
        }

        cartDto.DiscountAmount = cartDto.Subtotal * discountRate;
        cartDto.FinalAmount = cartDto.Subtotal - cartDto.DiscountAmount;

        return cartDto;
    }

    // POST: api/Cart
    [HttpPost]
    public async Task<ActionResult<CartItemDto>> AddToCart(AddCartItemDto addCartItemDto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        // Check if book exists and is in stock
        var book = await _context.Books.FindAsync(addCartItemDto.BookId);
        if (book == null)
        {
            return NotFound("Book not found");
        }

        if (book.StockQuantity < addCartItemDto.Quantity)
        {
            return BadRequest("Requested quantity exceeds available stock");
        }

        // Check if item already in cart
        var existingCartItem = await _context.CartItems
            .FirstOrDefaultAsync(c => c.UserId == userId && c.BookId == addCartItemDto.BookId);

        if (existingCartItem != null)
        {
            // Update quantity
            existingCartItem.Quantity += addCartItemDto.Quantity;
            existingCartItem.ModifiedAt = DateTime.UtcNow;

            // Check stock again after adding
            if (existingCartItem.Quantity > book.StockQuantity)
            {
                return BadRequest("Total quantity exceeds available stock");
            }

            await _context.SaveChangesAsync();

            return Ok(new CartItemDto
            {
                Id = existingCartItem.Id,
                BookId = book.Id,
                BookTitle = book.Title,
                BookAuthor = book.Author,
                BookImageUrl = book.ImageUrl,
                Quantity = existingCartItem.Quantity,
                UnitPrice = book.Price,
                DiscountPercentage = book.IsOnSale &&
                                  book.DiscountPercentage.HasValue &&
                                  book.DiscountStartDate <= DateTime.UtcNow &&
                                  (!book.DiscountEndDate.HasValue || book.DiscountEndDate >= DateTime.UtcNow)
                                  ? book.DiscountPercentage
                                  : null,
                LineTotal = CalculateLineTotal(book, existingCartItem.Quantity)
            });
        }

        // Add new cart item
        var cartItem = new CartItem
        {
            UserId = userId,
            BookId = addCartItemDto.BookId,
            Quantity = addCartItemDto.Quantity,
            CreatedAt = DateTime.UtcNow,
            ModifiedAt = DateTime.UtcNow
        };

        _context.CartItems.Add(cartItem);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetCartItem), new { id = cartItem.Id }, new CartItemDto
        {
            Id = cartItem.Id,
            BookId = book.Id,
            BookTitle = book.Title,
            BookAuthor = book.Author,
            BookImageUrl = book.ImageUrl,
            Quantity = cartItem.Quantity,
            UnitPrice = book.Price,
            DiscountPercentage = book.IsOnSale &&
                              book.DiscountPercentage.HasValue &&
                              book.DiscountStartDate <= DateTime.UtcNow &&
                              (!book.DiscountEndDate.HasValue || book.DiscountEndDate >= DateTime.UtcNow)
                              ? book.DiscountPercentage
                              : null,
            LineTotal = CalculateLineTotal(book, cartItem.Quantity)
        });
    }

    // GET: api/Cart/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<CartItemDto>> GetCartItem(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var cartItem = await _context.CartItems
            .Include(c => c.Book)
            .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

        if (cartItem == null)
        {
            return NotFound();
        }

        return new CartItemDto
        {
            Id = cartItem.Id,
            BookId = cartItem.BookId,
            BookTitle = cartItem.Book.Title,
            BookAuthor = cartItem.Book.Author,
            BookImageUrl = cartItem.Book.ImageUrl,
            Quantity = cartItem.Quantity,
            UnitPrice = cartItem.Book.Price,
            DiscountPercentage = cartItem.Book.IsOnSale &&
                              cartItem.Book.DiscountPercentage.HasValue &&
                              cartItem.Book.DiscountStartDate <= DateTime.UtcNow &&
                              (!cartItem.Book.DiscountEndDate.HasValue || cartItem.Book.DiscountEndDate >= DateTime.UtcNow)
                              ? cartItem.Book.DiscountPercentage
                              : null,
            LineTotal = CalculateLineTotal(cartItem.Book, cartItem.Quantity)
        };
    }

    // PUT: api/Cart/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCartItem(int id, UpdateCartItemDto updateCartItemDto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var cartItem = await _context.CartItems
            .Include(c => c.Book)
            .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

        if (cartItem == null)
        {
            return NotFound();
        }

        // Check stock
        if (updateCartItemDto.Quantity > cartItem.Book.StockQuantity)
        {
            return BadRequest("Requested quantity exceeds available stock");
        }

        cartItem.Quantity = updateCartItemDto.Quantity;
        cartItem.ModifiedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/Cart/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> RemoveFromCart(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var cartItem = await _context.CartItems
            .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

        if (cartItem == null)
        {
            return NotFound();
        }

        _context.CartItems.Remove(cartItem);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // Helper method to calculate line total with discounts
    private decimal CalculateLineTotal(Book book, int quantity)
    {
        decimal price = book.Price;

        // Apply book-specific discount if applicable
        if (book.IsOnSale &&
            book.DiscountPercentage.HasValue &&
            book.DiscountStartDate <= DateTime.UtcNow &&
            (!book.DiscountEndDate.HasValue || book.DiscountEndDate >= DateTime.UtcNow))
        {
            price -= price * (book.DiscountPercentage.Value / 100m);
        }

        return price * quantity;
    }
}
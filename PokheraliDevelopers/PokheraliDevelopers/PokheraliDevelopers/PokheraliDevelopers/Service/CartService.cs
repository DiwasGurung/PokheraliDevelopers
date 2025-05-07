using Microsoft.EntityFrameworkCore;
using PokheraliDevelopers.Data;
using PokheraliDevelopers.Models;
using PokheraliDevelopers.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PokheraliDevelopers.Services
{
    public class CartService
    {
        private readonly ApplicationDbContext _context;

        public CartService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CartDto> GetCartAsync(string userId)
        {
            var cartItems = await _context.CartItems
                .Where(ci => ci.UserId == userId)
                .Include(ci => ci.Book)
                .ToListAsync();

            var cartDto = new CartDto
            {
                Items = cartItems.Select(ci => new CartItemDto
                {
                    Id = ci.Id,
                    BookId = ci.BookId,
                    BookTitle = ci.Book.Title,
                    BookAuthor = ci.Book.Author,
                    BookImageUrl = ci.Book.ImageUrl,
                    BookPrice = ci.Book.Price,
                    Quantity = ci.Quantity,
                    Subtotal = ci.Book.Price * ci.Quantity
                }).ToList(),
                TotalItems = cartItems.Sum(ci => ci.Quantity),
                TotalPrice = cartItems.Sum(ci => ci.Book.Price * ci.Quantity)
            };

            return cartDto;
        }

        public async Task<CartItemDto> AddToCartAsync(string userId, AddToCartDto addToCartDto)
        {
            // Check if the book exists
            var book = await _context.Books.FindAsync(addToCartDto.BookId);
            if (book == null)
            {
                throw new ArgumentException("Book not found");
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

                return new CartItemDto
                {
                    Id = existingCartItem.Id,
                    BookId = existingCartItem.BookId,
                    BookTitle = book.Title,
                    BookAuthor = book.Author,
                    BookImageUrl = book.ImageUrl,
                    BookPrice = book.Price,
                    Quantity = existingCartItem.Quantity,
                    Subtotal = book.Price * existingCartItem.Quantity
                };
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

                return new CartItemDto
                {
                    Id = newCartItem.Id,
                    BookId = newCartItem.BookId,
                    BookTitle = book.Title,
                    BookAuthor = book.Author,
                    BookImageUrl = book.ImageUrl,
                    BookPrice = book.Price,
                    Quantity = newCartItem.Quantity,
                    Subtotal = book.Price * newCartItem.Quantity
                };
            }
        }

        public async Task<bool> UpdateCartItemAsync(string userId, UpdateCartItemDto updateCartItemDto)
        {
            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(ci => ci.Id == updateCartItemDto.CartItemId && ci.UserId == userId);

            if (cartItem == null)
            {
                return false;
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
                var book = await _context.Books.FindAsync(cartItem.BookId);

                // Check if quantity exceeds stock
                if (book.Stock > 0 && updateCartItemDto.Quantity > book.Stock)
                {
                    cartItem.Quantity = book.Stock;
                }
                else
                {
                    cartItem.Quantity = updateCartItemDto.Quantity;
                }

                _context.CartItems.Update(cartItem);
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveFromCartAsync(string userId, int cartItemId)
        {
            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(ci => ci.Id == cartItemId && ci.UserId == userId);

            if (cartItem == null)
            {
                return false;
            }

            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task ClearCartAsync(string userId)
        {
            var cartItems = await _context.CartItems
                .Where(ci => ci.UserId == userId)
                .ToListAsync();

            _context.CartItems.RemoveRange(cartItems);
            await _context.SaveChangesAsync();
        }
    }
}
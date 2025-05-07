using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PokheraliDevelopers.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class BookmarksController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public BookmarksController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/Bookmarks - Get all bookmarks for the logged-in user
    [HttpGet]
    [Authorize(Roles = "Member")]
    public async Task<ActionResult<IEnumerable<BookResponseDto>>> GetUserBookmarks()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var bookmarks = await _context.Bookmarks
            .Include(b => b.Book)
            .Where(b => b.UserId == userId)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();

        var bookDtos = bookmarks.Select(b => new BookResponseDto
        {
            Id = b.Book.Id,
            Title = b.Book.Title,
            Author = b.Book.Author,
            Description = b.Book.Description,
            Price = b.Book.Price,
            Stock = b.Book.Stock,
            Genre = b.Book.Genre,
            ISBN = b.Book.ISBN,
            Publisher = b.Book.Publisher,
            PublishDate = b.Book.PublishDate,
            Language = b.Book.Language,
            Format = b.Book.Format,
            Pages = b.Book.Pages,
            Dimensions = b.Book.Dimensions,
            Weight = b.Book.Weight,
            ImageUrl = b.Book.ImageUrl,
            CreatedAt = b.Book.CreatedAt,
            UpdatedAt = b.Book.UpdatedAt
        }).ToList();

        return bookDtos;
    }

    // POST: api/Bookmarks - Add a book to bookmarks
    [HttpPost]
    [Authorize(Roles = "Member")]
    public async Task<IActionResult> AddBookmark([FromBody] int bookId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        // Check if book exists
        var book = await _context.Books.FindAsync(bookId);
        if (book == null)
        {
            return NotFound("Book not found");
        }

        // Check if bookmark already exists
        var existingBookmark = await _context.Bookmarks
            .FirstOrDefaultAsync(b => b.BookId == bookId && b.UserId == userId);

        if (existingBookmark != null)
        {
            return BadRequest("Book is already in your bookmarks");
        }

        // Create bookmark
        var bookmark = new Bookmark
        {
            BookId = bookId,
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        };

        _context.Bookmarks.Add(bookmark);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Book added to bookmarks" });
    }

    // DELETE: api/Bookmarks/{bookId} - Remove a book from bookmarks
    [HttpDelete("{bookId}")]
    [Authorize(Roles = "Member")]
    public async Task<IActionResult> RemoveBookmark(int bookId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var bookmark = await _context.Bookmarks
            .FirstOrDefaultAsync(b => b.BookId == bookId && b.UserId == userId);

        if (bookmark == null)
        {
            return NotFound("Bookmark not found");
        }

        _context.Bookmarks.Remove(bookmark);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Book removed from bookmarks" });
    }
}
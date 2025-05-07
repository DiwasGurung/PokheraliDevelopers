// Controllers/BookmarksController.cs (updated)
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
public class BookmarksController : ControllerBase
{
    private readonly DatabaseHandlerEfCoreExample _context;

    public BookmarksController(DatabaseHandlerEfCoreExample context)
    {
        _context = context;
    }

    // GET: api/Bookmarks
    [HttpGet]
    public async Task<ActionResult<IEnumerable<BookmarkDto>>> GetBookmarks()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var bookmarks = await _context.Bookmarks
            .Where(b => b.UserId == userId)
            .Include(b => b.Book)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();

        return bookmarks.Select(b => new BookmarkDto
        {
            Id = b.Id,
            BookId = b.BookId,
            BookTitle = b.Book.Title,
            BookAuthor = b.Book.Author,
            BookImageUrl = b.Book.ImageUrl,
            BookPrice = b.Book.Price,
            BookDiscountPercentage = b.Book.DiscountPercentage,
            CreatedAt = b.CreatedAt
        }).ToList();
    }

    // POST: api/Bookmarks
    [HttpPost]
    public async Task<ActionResult<BookmarkDto>> AddBookmark(int bookId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        // Check if book exists
        var book = await _context.Books.FindAsync(bookId);
        if (book == null)
        {
            return NotFound("Book not found");
        }

        // Check if already bookmarked
        var existingBookmark = await _context.Bookmarks
            .FirstOrDefaultAsync(b => b.UserId == userId && b.BookId == bookId);

        if (existingBookmark != null)
        {
            return Conflict("Book already bookmarked");
        }

        var bookmark = new Bookmark
        {
            UserId = userId,
            BookId = bookId,
            CreatedAt = DateTime.UtcNow
        };

        _context.Bookmarks.Add(bookmark);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetBookmark), new { id = bookmark.Id }, new BookmarkDto
        {
            Id = bookmark.Id,
            BookId = book.Id,
            BookTitle = book.Title,
            BookAuthor = book.Author,
            BookImageUrl = book.ImageUrl,
            BookPrice = book.Price,
            // Continuing BookmarksController.cs from where we left off
            BookDiscountPercentage = book.DiscountPercentage,
            CreatedAt = bookmark.CreatedAt
        });
    }

    // GET: api/Bookmarks/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<BookmarkDto>> GetBookmark(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var bookmark = await _context.Bookmarks
            .Include(b => b.Book)
            .FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);

        if (bookmark == null)
        {
            return NotFound();
        }

        return new BookmarkDto
        {
            Id = bookmark.Id,
            BookId = bookmark.BookId,
            BookTitle = bookmark.Book.Title,
            BookAuthor = bookmark.Book.Author,
            BookImageUrl = bookmark.Book.ImageUrl,
            BookPrice = bookmark.Book.Price,
            BookDiscountPercentage = bookmark.Book.DiscountPercentage,
            CreatedAt = bookmark.CreatedAt
        };
    }

    // DELETE: api/Bookmarks/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBookmark(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var bookmark = await _context.Bookmarks
            .FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);

        if (bookmark == null)
        {
            return NotFound();
        }

        _context.Bookmarks.Remove(bookmark);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
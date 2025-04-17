using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly DatabaseHandlerEfCoreExample _context;

    public BooksController(DatabaseHandlerEfCoreExample context)
    {
        _context = context;
    }

    // GET: api/Books
    [HttpGet]
    public async Task<ActionResult<IEnumerable<BookResponseDto>>> GetBooks()
    {
        var books = await _context.Books
            .Select(b => new BookResponseDto
            {
                Id = b.Id,
                Title = b.Title,
                Author = b.Author,
                Description = b.Description,
                Price = b.Price,
                Stock = b.Stock,
                Genre = b.Genre,
                ISBN = b.ISBN,
                Publisher = b.Publisher,
                PublishDate = b.PublishDate,
                Language = b.Language,
                Format = b.Format,
                Pages = b.Pages,
                Dimensions = b.Dimensions,
                Weight = b.Weight,
                ImageUrl = b.ImageUrl,
                CreatedAt = b.CreatedAt,
                UpdatedAt = b.UpdatedAt
            })
            .ToListAsync();

        return books;
    }

    // GET: api/Books/5
    [HttpGet("{id}")]
    public async Task<ActionResult<BookResponseDto>> GetBook(int id)
    {
        var book = await _context.Books.FindAsync(id);

        if (book == null)
        {
            return NotFound();
        }

        var bookDto = new BookResponseDto
        {
            Id = book.Id,
            Title = book.Title,
            Author = book.Author,
            Description = book.Description,
            Price = book.Price,
            Stock = book.Stock,
            Genre = book.Genre,
            ISBN = book.ISBN,
            Publisher = book.Publisher,
            PublishDate = book.PublishDate,
            Language = book.Language,
            Format = book.Format,
            Pages = book.Pages,
            Dimensions = book.Dimensions,
            Weight = book.Weight,
            ImageUrl = book.ImageUrl,
            CreatedAt = book.CreatedAt,
            UpdatedAt = book.UpdatedAt
        };

        return bookDto;
    }

    // POST: api/Books
    [HttpPost]
      // Restrict to admin and staff roles
    public async Task<ActionResult<BookResponseDto>> CreateBook([FromBody] BookDto bookDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var book = new Book
        {
            Title = bookDto.Title,
            Author = bookDto.Author,
            Description = bookDto.Description,
            Price = bookDto.Price,
            Stock = bookDto.Stock,
            Genre = bookDto.Genre,
            ISBN = bookDto.ISBN,
            Publisher = bookDto.Publisher,
            PublishDate = bookDto.PublishDate,
            Language = bookDto.Language,
            Format = bookDto.Format,
            Pages = bookDto.Pages,
            Dimensions = bookDto.Dimensions,
            Weight = bookDto.Weight,
            ImageUrl = bookDto.ImageUrl,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Books.Add(book);
        await _context.SaveChangesAsync();

        var responseDto = new BookResponseDto
        {
            Id = book.Id,
            Title = book.Title,
            Author = book.Author,
            Description = book.Description,
            Price = book.Price,
            Stock = book.Stock,
            Genre = book.Genre,
            ISBN = book.ISBN,
            Publisher = book.Publisher,
            PublishDate = book.PublishDate,
            Language = book.Language,
            Format = book.Format,
            Pages = book.Pages,
            Dimensions = book.Dimensions,
            Weight = book.Weight,
            ImageUrl = book.ImageUrl,
            CreatedAt = book.CreatedAt,
            UpdatedAt = book.UpdatedAt
        };

        return CreatedAtAction(nameof(GetBook), new { id = book.Id }, responseDto);
    }

    // PUT: api/Books/5
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin, Staff")]
    public async Task<IActionResult> UpdateBook(int id, [FromBody] BookDto bookDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var book = await _context.Books.FindAsync(id);
        if (book == null)
        {
            return NotFound();
        }

        // Update all properties
        book.Title = bookDto.Title;
        book.Author = bookDto.Author;
        book.Description = bookDto.Description;
        book.Price = bookDto.Price;
        book.Stock = bookDto.Stock;
        book.Genre = bookDto.Genre;
        book.ISBN = bookDto.ISBN;
        book.Publisher = bookDto.Publisher;
        book.PublishDate = bookDto.PublishDate;
        book.Language = bookDto.Language;
        book.Format = bookDto.Format;
        book.Pages = bookDto.Pages;
        book.Dimensions = bookDto.Dimensions;
        book.Weight = bookDto.Weight;
        book.ImageUrl = bookDto.ImageUrl;
        book.UpdatedAt = DateTime.UtcNow;

        _context.Entry(book).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!BookExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    // DELETE: api/Books/5
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]  // Restrict deletion to admin only
    public async Task<IActionResult> DeleteBook(int id)
    {
        var book = await _context.Books.FindAsync(id);
        if (book == null)
        {
            return NotFound();
        }

        _context.Books.Remove(book);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool BookExists(int id)
    {
        return _context.Books.Any(e => e.Id == id);
    }
}
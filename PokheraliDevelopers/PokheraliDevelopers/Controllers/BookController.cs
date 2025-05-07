﻿// Controllers/BooksController.cs (updated)
using PokheraliDevelopers.Dto;
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
public class BooksController : ControllerBase
{
    private readonly DatabaseHandlerEfCoreExample _context;

    public BooksController(DatabaseHandlerEfCoreExample context)
    {
        _context = context;
    }

    // GET: api/Books
    [HttpGet]
    public async Task<ActionResult<PaginatedResponseDto<BookDto>>> GetBooks([FromQuery] BookFilterDto filter)
    {
        var query = _context.Books.AsQueryable();

        // Apply search term filter
        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
        {
            var searchTerm = filter.SearchTerm.ToLower();
            query = query.Where(b =>
                b.Title.ToLower().Contains(searchTerm) ||
                b.Author.ToLower().Contains(searchTerm) ||
                b.ISBN.ToLower().Contains(searchTerm) ||
                b.Description.ToLower().Contains(searchTerm));
        }

        // Apply other filters
        if (filter.Authors != null && filter.Authors.Any())
            query = query.Where(b => filter.Authors.Contains(b.Author));

        if (filter.Genres != null && filter.Genres.Any())
            query = query.Where(b => filter.Genres.Contains(b.Genre));

        if (filter.InStock.HasValue)
            query = query.Where(b => filter.InStock.Value ? b.StockQuantity > 0 : b.StockQuantity == 0);

        if (filter.MinPrice.HasValue)
            query = query.Where(b => b.Price >= filter.MinPrice.Value);

        if (filter.MaxPrice.HasValue)
            query = query.Where(b => b.Price <= filter.MaxPrice.Value);

        if (filter.Languages != null && filter.Languages.Any())
            query = query.Where(b => filter.Languages.Contains(b.Language));

        if (filter.Formats != null && filter.Formats.Any())
            query = query.Where(b => filter.Formats.Contains(b.Format));

        if (filter.Publishers != null && filter.Publishers.Any())
            query = query.Where(b => filter.Publishers.Contains(b.Publisher));

        if (filter.OnSale.HasValue && filter.OnSale.Value)
            query = query.Where(b => b.IsOnSale &&
                                    b.DiscountPercentage.HasValue &&
                                    b.DiscountStartDate <= DateTime.UtcNow &&
                                    (!b.DiscountEndDate.HasValue || b.DiscountEndDate >= DateTime.UtcNow));

        if (filter.NewRelease.HasValue && filter.NewRelease.Value)
            query = query.Where(b => b.PublicationDate >= DateTime.UtcNow.AddMonths(-3));

        if (filter.NewArrival.HasValue && filter.NewArrival.Value)
            query = query.Where(b => b.CreatedAt >= DateTime.UtcNow.AddMonths(-1));

        if (filter.ComingSoon.HasValue && filter.ComingSoon.Value)
            query = query.Where(b => b.PublicationDate > DateTime.UtcNow);

        if (filter.AwardWinner.HasValue && filter.AwardWinner.Value)
            query = query.Where(b => b.BookAwards.Any());

        if (filter.MinRating.HasValue)
            query = query.Where(b => b.Reviews.Count > 0 && b.Reviews.Average(r => r.Rating) >= filter.MinRating.Value);

        // Calculate the total count before applying pagination
        var totalCount = await query.CountAsync();

        // Apply sorting
        query = filter.SortBy?.ToLower() switch
        {
            "title" => filter.SortDescending ? query.OrderByDescending(b => b.Title) : query.OrderBy(b => b.Title),
            "author" => filter.SortDescending ? query.OrderByDescending(b => b.Author) : query.OrderBy(b => b.Author),
            "price" => filter.SortDescending ? query.OrderByDescending(b => b.Price) : query.OrderBy(b => b.Price),
            "publicationdate" => filter.SortDescending ? query.OrderByDescending(b => b.PublicationDate) : query.OrderBy(b => b.PublicationDate),
            "popularity" => query.OrderByDescending(b => b.OrderItems.Count),
            _ => query.OrderBy(b => b.Title)
        };

        // Apply pagination
        var pageSize = filter.PageSize > 0 ? filter.PageSize : 10;
        var pageNumber = filter.PageNumber > 0 ? filter.PageNumber : 1;
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        var books = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        // Get current user id if authenticated
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var isAuthenticated = !string.IsNullOrEmpty(userId);

        // Map to DTOs
        var bookDtos = books.Select(b => new BookDto
        {
            Id = b.Id,
            Title = b.Title,
            ISBN = b.ISBN,
            Description = b.Description,
            Author = b.Author,
            Publisher = b.Publisher,
            PublicationDate = b.PublicationDate,
            Price = b.Price,
            StockQuantity = b.StockQuantity,
            Language = b.Language,
            Format = b.Format,
            Genre = b.Genre,
            ImageUrl = b.ImageUrl,
            DiscountPercentage = b.DiscountPercentage,
            IsOnSale = b.IsOnSale &&
                      b.DiscountPercentage.HasValue &&
                      b.DiscountStartDate <= DateTime.UtcNow &&
                      (!b.DiscountEndDate.HasValue || b.DiscountEndDate >= DateTime.UtcNow),
            IsNewRelease = b.PublicationDate >= DateTime.UtcNow.AddMonths(-3),
            IsNewArrival = b.CreatedAt >= DateTime.UtcNow.AddMonths(-1),
            IsComingSoon = b.PublicationDate > DateTime.UtcNow,
            AverageRating = b.Reviews.Any() ? b.Reviews.Average(r => r.Rating) : 0,
            ReviewCount = b.Reviews.Count,
            IsBookmarked = isAuthenticated && b.Bookmarks.Any(bm => bm.UserId == userId),
            Awards = b.BookAwards.Select(ba => ba.Award.Name).ToList()
        }).ToList();

        return new PaginatedResponseDto<BookDto>
        {
            Items = bookDtos,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = totalPages
        };
    }

    // GET: api/Books/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<BookDto>> GetBook(int id)
    {
        var book = await _context.Books
            .Include(b => b.Reviews)
            .Include(b => b.BookAwards)
                .ThenInclude(ba => ba.Award)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (book == null)
        {
            return NotFound();
        }

        // Get current user id if authenticated
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var isAuthenticated = !string.IsNullOrEmpty(userId);

        var bookDto = new BookDto
        {
            Id = book.Id,
            Title = book.Title,
            ISBN = book.ISBN,
            Description = book.Description,
            Author = book.Author,
            Publisher = book.Publisher,
            PublicationDate = book.PublicationDate,
            Price = book.Price,
            StockQuantity = book.StockQuantity,
            Language = book.Language,
            Format = book.Format,
            Genre = book.Genre,
            ImageUrl = book.ImageUrl,
            DiscountPercentage = book.DiscountPercentage,
            IsOnSale = book.IsOnSale &&
                      book.DiscountPercentage.HasValue &&
                      book.DiscountStartDate <= DateTime.UtcNow &&
                      (!book.DiscountEndDate.HasValue || book.DiscountEndDate >= DateTime.UtcNow),
            IsNewRelease = book.PublicationDate >= DateTime.UtcNow.AddMonths(-3),
            IsNewArrival = book.CreatedAt >= DateTime.UtcNow.AddMonths(-1),
            IsComingSoon = book.PublicationDate > DateTime.UtcNow,
            AverageRating = book.Reviews.Any() ? book.Reviews.Average(r => r.Rating) : 0,
            ReviewCount = book.Reviews.Count,
            IsBookmarked = isAuthenticated && _context.Bookmarks.Any(bm => bm.BookId == id && bm.UserId == userId),
            Awards = book.BookAwards.Select(ba => ba.Award.Name).ToList()
        };

        return bookDto;
    }

    // Admin methods (shortened for brevity)
    // For Admin: Create a new book
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<BookDto>> CreateBook(CreateBookDto createBookDto)
    {
        var book = new Book
        {
            Title = createBookDto.Title,
            ISBN = createBookDto.ISBN,
            Description = createBookDto.Description,
            Author = createBookDto.Author,
            Publisher = createBookDto.Publisher,
            PublicationDate = createBookDto.PublicationDate,
            Price = createBookDto.Price,
            StockQuantity = createBookDto.StockQuantity,
            Language = createBookDto.Language,
            Format = createBookDto.Format,
            Genre = createBookDto.Genre,
            ImageUrl = createBookDto.ImageUrl,
            DiscountPercentage = createBookDto.DiscountPercentage,
            IsOnSale = createBookDto.IsOnSale,
            DiscountStartDate = createBookDto.DiscountStartDate,
            DiscountEndDate = createBookDto.DiscountEndDate,
            CreatedAt = DateTime.UtcNow,
            ModifiedAt = DateTime.UtcNow
        };

        _context.Books.Add(book);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetBook), new { id = book.Id }, new BookDto
        {
            Id = book.Id,
            Title = book.Title,
            ISBN = book.ISBN,
            Description = book.Description,
            Author = book.Author,
            Publisher = book.Publisher,
            PublicationDate = book.PublicationDate,
            Price = book.Price,
            StockQuantity = book.StockQuantity,
            Language = book.Language,
            Format = book.Format,
            Genre = book.Genre,
            ImageUrl = book.ImageUrl,
            DiscountPercentage = book.DiscountPercentage,
            IsOnSale = book.IsOnSale,
            IsNewRelease = book.PublicationDate >= DateTime.UtcNow.AddMonths(-3),
            IsNewArrival = true,
            IsComingSoon = book.PublicationDate > DateTime.UtcNow,
        });
    }

    // PUT and DELETE methods for admin would follow here
}




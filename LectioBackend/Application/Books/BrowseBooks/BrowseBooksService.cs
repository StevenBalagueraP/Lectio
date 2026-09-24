using Lectio.Api.Application.Books.BrowseBooks.Dtos;
using Lectio.Api.Application.Books.TopRatedBooks.Dtos;
using Lectio.Api.Domain.Entities;
using Lectio.Api.Domain.Exceptions;
using Lectio.Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Lectio.Api.Application.Books.BrowseBooks;

public class BrowseBooksService : IBrowseBooksService
{
    private const int MaxPageSize = 50;

    private readonly AppDbContext _context;

    public BrowseBooksService(AppDbContext context)
    {
        _context = context;
    }

        public async Task<BrowseBooksResponseDto> GetBooksAsync(
        string? search,
        int? categoryId,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        ValidatePaging(page, pageSize);

        if (categoryId.HasValue)
        {
            await EnsureCategoryExistsAsync(categoryId.Value, cancellationToken);
        }

        var query = BuildFilteredQuery(search, categoryId);

        var totalCount = await query.CountAsync(cancellationToken);

        var books = await query
            .OrderBy(book => book.Title)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var bookIds = books.Select(book => book.BookId).ToList();
        var authorsByBookId = await GetAuthorsByBookIdAsync(bookIds, cancellationToken);
        var categoriesByBookId = await GetCategoriesByBookIdAsync(bookIds, cancellationToken);

        var data = books
            .Select(book => MapToDto(book, authorsByBookId, categoriesByBookId))
            .ToList();

        return new BrowseBooksResponseDto
        {
            Data = data,
            Pagination = new PaginationDto
            {
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            }
        };
    }

    private static void ValidatePaging(int page, int pageSize)
    {
        if (page <= 0)
        {
            throw new BadRequestException("page must be a positive integer.");
        }

        if (pageSize is <= 0 or > MaxPageSize)
        {
            throw new BadRequestException($"pageSize must be between 1 and {MaxPageSize}.");
        }
    }

    private async Task EnsureCategoryExistsAsync(int categoryId, CancellationToken cancellationToken)
    {
        var categoryExists = await _context.Categories
            .AsNoTracking()
            .AnyAsync(category => category.CategoryId == categoryId, cancellationToken);

        if (!categoryExists)
        {
            throw new NotFoundException(nameof(Category), categoryId);
        }
    }

    private IQueryable<Book> BuildFilteredQuery(string? search, int? categoryId)
    {
        var query = _context.Books
            .AsNoTracking()
            .Where(book => book.IsAvailable);

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(book => EF.Functions.ILike(book.Title, $"%{search}%"));
        }

        if (categoryId.HasValue)
        {
            query = query.Where(book => _context.BookCategories
                .Any(bookCategory => bookCategory.BookId == book.BookId && bookCategory.CategoryId == categoryId.Value));
        }

        return query;
    }

        private async Task<Dictionary<int, List<BookCategorySummaryDto>>> GetCategoriesByBookIdAsync(
        IReadOnlyCollection<int> bookIds,
        CancellationToken cancellationToken)
    {
        var bookCategories = await _context.BookCategories
            .AsNoTracking()
            .Where(bookCategory => bookIds.Contains(bookCategory.BookId))
            .OrderBy(bookCategory => bookCategory.CategoryId)
            .Select(bookCategory => new
            {
                bookCategory.BookId,
                bookCategory.CategoryId,
                bookCategory.Category.CategoryName
            })
            .ToListAsync(cancellationToken);

        return bookCategories
            .GroupBy(bookCategory => bookCategory.BookId)
            .ToDictionary(
                group => group.Key,
                group => group
                    .Select(bookCategory => new BookCategorySummaryDto { Id = bookCategory.CategoryId, Name = bookCategory.CategoryName })
                    .ToList());
    }

    private static BookSummaryDto MapToDto(
        Book book,
        IReadOnlyDictionary<int, List<BookAuthorSummaryDto>> authorsByBookId,
        IReadOnlyDictionary<int, List<BookCategorySummaryDto>> categoriesByBookId)
    {
        authorsByBookId.TryGetValue(book.BookId, out var authors);
        categoriesByBookId.TryGetValue(book.BookId, out var categories);

        return new BookSummaryDto
        {
            Id = book.BookId,
            Title = book.Title,
            ImageUrl = book.ImageLink,
            Rating = new BookRatingDto { Stars = book.AverageRating },
            Authors = authors ?? new List<BookAuthorSummaryDto>(),
            Categories = categories ?? new List<BookCategorySummaryDto>()
        };
    }

    private async Task<Dictionary<int, List<BookAuthorSummaryDto>>> GetAuthorsByBookIdAsync(
        IReadOnlyCollection<int> bookIds,
        CancellationToken cancellationToken)
    {
        var bookAuthors = await _context.BookAuthors
            .AsNoTracking()
            .Where(bookAuthor => bookIds.Contains(bookAuthor.BookId))
            .OrderBy(bookAuthor => bookAuthor.AuthorId)
            .Select(bookAuthor => new
            {
                bookAuthor.BookId,
                bookAuthor.AuthorId,
                bookAuthor.Author.AuthorName
            })
            .ToListAsync(cancellationToken);

        return bookAuthors
            .GroupBy(bookAuthor => bookAuthor.BookId)
            .ToDictionary(
                group => group.Key,
                group => group
                    .Select(bookAuthor => new BookAuthorSummaryDto { Id = bookAuthor.AuthorId, Name = bookAuthor.AuthorName })
                    .ToList());
    }

    private static BookSummaryDto MapToDto(Book book, IReadOnlyDictionary<int, List<BookAuthorSummaryDto>> authorsByBookId)
    {
        authorsByBookId.TryGetValue(book.BookId, out var authors);

        return new BookSummaryDto
        {
            Id = book.BookId,
            Title = book.Title,
            ImageUrl = book.ImageLink,
            Rating = new BookRatingDto { Stars = book.AverageRating },
            Authors = authors ?? new List<BookAuthorSummaryDto>()
        };
    }
}
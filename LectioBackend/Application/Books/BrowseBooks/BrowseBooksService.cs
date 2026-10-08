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
    IReadOnlyCollection<int>? categoryIds,
    int page,
    int pageSize,
    CancellationToken cancellationToken)
{
    ValidatePaging(page, pageSize);

    var normalizedCategoryIds = NormalizeCategoryIds(categoryIds);

    if (normalizedCategoryIds.Count > 0)
    {
        await EnsureCategoriesExistAsync(normalizedCategoryIds, cancellationToken);
    }

    var query = BuildFilteredQuery(search, normalizedCategoryIds);

    var totalCount = await query.CountAsync(cancellationToken);

    var rows = await query
        .OrderBy(book => book.Title)
        .ThenBy(book => book.BookId) // desempate: títulos repetidos no deben saltarse ni duplicarse entre páginas
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .Select(book => new
        {
            book.BookId,
            book.Title,
            book.ImageLink,
            book.AverageRating,
            Authors = _context.BookAuthors
                .Where(bookAuthor => bookAuthor.BookId == book.BookId)
                .OrderBy(bookAuthor => bookAuthor.AuthorId)
                .Select(bookAuthor => new { bookAuthor.AuthorId, bookAuthor.Author.AuthorName })
                .ToList(),
            Categories = _context.BookCategories
                .Where(bookCategory => bookCategory.BookId == book.BookId)
                .OrderBy(bookCategory => bookCategory.CategoryId)
                .Select(bookCategory => new { bookCategory.CategoryId, bookCategory.Category.CategoryName })
                .ToList()
        })
        .ToListAsync(cancellationToken);

    var data = rows
        .Select(row => new BookSummaryDto
        {
            Id = row.BookId,
            Title = row.Title,
            ImageUrl = row.ImageLink,
            Rating = new BookRatingDto { Stars = row.AverageRating },
            Authors = row.Authors
                .Select(author => new BookAuthorSummaryDto { Id = author.AuthorId, Name = author.AuthorName })
                .ToList(),
            Categories = row.Categories
                .Select(category => new BookCategorySummaryDto { Id = category.CategoryId, Name = category.CategoryName })
                .ToList()
        })
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

    private static List<int> NormalizeCategoryIds(IReadOnlyCollection<int>? categoryIds)
    {
        if (categoryIds is null || categoryIds.Count == 0)
        {
            return new List<int>();
        }

        var normalized = categoryIds.Distinct().ToList();

        if (normalized.Any(categoryId => categoryId <= 0))
        {
            throw new BadRequestException("categoryIds must contain only positive integers.");
        }

        return normalized;
    }

    private async Task EnsureCategoriesExistAsync(List<int> categoryIds, CancellationToken cancellationToken)
    {
        var existingIds = await _context.Categories
            .AsNoTracking()
            .Where(category => categoryIds.Contains(category.CategoryId))
            .Select(category => category.CategoryId)
            .ToListAsync(cancellationToken);

        var missingIds = categoryIds.Except(existingIds).ToList();

        if (missingIds.Count > 0)
        {
            throw new NotFoundException(nameof(Category), missingIds.First());
        }
    }

    private IQueryable<Book> BuildFilteredQuery(string? search, IReadOnlyCollection<int> categoryIds)
    {
        var query = _context.Books
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(book => EF.Functions.ILike(book.Title, $"%{search}%"));
        }

        if (categoryIds.Count > 0)
        {
            query = query.Where(book => _context.BookCategories
                .Any(bookCategory => bookCategory.BookId == book.BookId && categoryIds.Contains(bookCategory.CategoryId)));
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
}
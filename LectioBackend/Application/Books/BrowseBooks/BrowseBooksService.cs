using Lectio.Api.Application.Books.BrowseBooks.Dtos;
using Lectio.Api.Application.Books.TopRatedBooks.Dtos;
using Lectio.Api.Domain.Entities;
using Lectio.Api.Domain.Exceptions;
using Lectio.Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

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
        IReadOnlyCollection<BookFilter>? filters,
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

        var normalizedFilters = NormalizeFilters(filters);

        var query = BuildFilteredQuery(search, normalizedCategoryIds, normalizedFilters);

        var totalCount = await query.CountAsync(cancellationToken);

        var rows = await ApplyOrdering(query, normalizedFilters)
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

    private IQueryable<Book> BuildFilteredQuery(
        string? search,
        IReadOnlyCollection<int> categoryIds,
        IReadOnlyList<BookFilter> filters)
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

        if (filters.Contains(BookFilter.Available))
        {
            query = query.Where(book => book.IsAvailable);
        }

        return query;
    }

    /// <summary>
    /// Quita duplicados (conservando el orden de llegada, que define la prioridad),
    /// valida valores desconocidos y rechaza combinaciones contradictorias.
    /// </summary>
    private static List<BookFilter> NormalizeFilters(IReadOnlyCollection<BookFilter>? filters)
    {
        if (filters is null || filters.Count == 0)
        {
            return new List<BookFilter>();
        }

        var normalized = filters.Distinct().ToList();

        if (normalized.Any(filter => !Enum.IsDefined(filter)))
        {
            throw new BadRequestException(
                "filter must be one of: TopRated, Available, Newest, Oldest, Longest, Shortest.");
        }

        if (normalized.Contains(BookFilter.Newest) && normalized.Contains(BookFilter.Oldest))
        {
            throw new BadRequestException("filter cannot combine Newest with Oldest.");
        }

        if (normalized.Contains(BookFilter.Longest) && normalized.Contains(BookFilter.Shortest))
        {
            throw new BadRequestException("filter cannot combine Longest with Shortest.");
        }

        return normalized;
    }

    /// <summary>
    /// Encadena los órdenes en el mismo orden en que llegaron (el primero es el principal).
    /// Available no ordena, solo filtra. Para fecha y páginas, los null van siempre al final:
    /// en PostgreSQL un DESC pondría los NULL primero, por eso se ordena antes por "IS NULL".
    /// Siempre cierra con título y BookId. Sin criterios de orden, queda por título A→Z como antes.
    /// </summary>
    private static IOrderedQueryable<Book> ApplyOrdering(IQueryable<Book> query, IReadOnlyList<BookFilter> filters)
    {
        IOrderedQueryable<Book>? ordered = null;

        foreach (var filter in filters)
        {
            switch (filter)
            {
                case BookFilter.TopRated:
                    ordered = AddOrder(query, ordered, book => book.AverageRating, descending: true);
                    break;

                case BookFilter.Newest:
                    ordered = AddOrder(query, ordered, book => book.PublishedDate == null, descending: false);
                    ordered = AddOrder(query, ordered, book => book.PublishedDate, descending: true);
                    break;

                case BookFilter.Oldest:
                    ordered = AddOrder(query, ordered, book => book.PublishedDate == null, descending: false);
                    ordered = AddOrder(query, ordered, book => book.PublishedDate, descending: false);
                    break;

                case BookFilter.Longest:
                    ordered = AddOrder(query, ordered, book => book.PageCount == null, descending: false);
                    ordered = AddOrder(query, ordered, book => book.PageCount, descending: true);
                    break;

                case BookFilter.Shortest:
                    ordered = AddOrder(query, ordered, book => book.PageCount == null, descending: false);
                    ordered = AddOrder(query, ordered, book => book.PageCount, descending: false);
                    break;
            }
        }

        ordered = AddOrder(query, ordered, book => book.Title, descending: false);

        return ordered;
    }

    private static IOrderedQueryable<Book> AddOrder<TKey>(
        IQueryable<Book> query,
        IOrderedQueryable<Book>? ordered,
        Expression<Func<Book, TKey>> keySelector,
        bool descending)
    {
        if (ordered is null)
        {
            return descending ? query.OrderByDescending(keySelector) : query.OrderBy(keySelector);
        }

        return descending ? ordered.ThenByDescending(keySelector) : ordered.ThenBy(keySelector);
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
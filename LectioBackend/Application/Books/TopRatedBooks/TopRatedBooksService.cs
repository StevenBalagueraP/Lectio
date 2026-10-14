using Lectio.Api.Application.Books.TopRatedBooks.Dtos;
using Lectio.Api.Domain.Entities;
using Lectio.Api.Domain.Exceptions;
using Lectio.Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Lectio.Api.Application.Books.TopRatedBooks;

public class TopRatedBooksService : ITopRatedBooksService
{
    private const int TopBooksCount = 5;

    private readonly AppDbContext _context;

    public TopRatedBooksService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<TopRatedBooksResponseDto> GetTopRatedBooksAsync(int? categoryId, CancellationToken cancellationToken)
    {
        ValidateCategoryId(categoryId);

        if (categoryId.HasValue)
        {
            await EnsureCategoryExistsAsync(categoryId.Value, cancellationToken);
        }

        var topBooks = await BuildTopRatedBooksQuery(categoryId).ToListAsync(cancellationToken);

        if (topBooks.Count == 0)
        {
            return new TopRatedBooksResponseDto();
        }

        var bookIds = topBooks.Select(book => book.BookId).ToList();

        var authorsByBookId = await GetAuthorsByBookIdAsync(bookIds, cancellationToken);
        var categoriesByBookId = await GetCategoriesByBookIdAsync(bookIds, cancellationToken);

        var data = topBooks
            .Select(book => MapToDto(book, authorsByBookId, categoriesByBookId))
            .ToList();

        return new TopRatedBooksResponseDto { Data = data };
    }

    private static void ValidateCategoryId(int? categoryId)
    {
        if (categoryId is <= 0)
        {
            throw new BadRequestException("categoryId must be a positive integer.");
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

    private IQueryable<Book> BuildTopRatedBooksQuery(int? categoryId)
    {
        var query = _context.Books
            .AsNoTracking()
            .Where(book => book.IsAvailable);

        if (categoryId.HasValue)
        {
            query = query.Where(book => _context.BookCategories
                .Any(bookCategory => bookCategory.BookId == book.BookId && bookCategory.CategoryId == categoryId.Value));
        }

        return query
            .OrderByDescending(book => book.AverageRating)
            .ThenByDescending(book => book.RatingCount)
            .Take(TopBooksCount);
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

    private static TopRatedBookDto MapToDto(
        Book book,
        IReadOnlyDictionary<int, List<BookAuthorSummaryDto>> authorsByBookId,
        IReadOnlyDictionary<int, List<BookCategorySummaryDto>> categoriesByBookId)
    {
        authorsByBookId.TryGetValue(book.BookId, out var authors);
        categoriesByBookId.TryGetValue(book.BookId, out var categories);

        return new TopRatedBookDto
        {
            Id = book.BookId,
            Title = book.Title,
            Description = book.Description,
            ImageUrl = book.ImageLink,
            Rating = new BookRatingDto { Stars = book.AverageRating },
            Authors = authors ?? new List<BookAuthorSummaryDto>(),
            Categories = categories ?? new List<BookCategorySummaryDto>()
        };
    }
}
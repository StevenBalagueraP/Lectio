using Lectio.Api.Application.Books.BookDetails.Dtos;
using Lectio.Api.Domain.Entities;
using Lectio.Api.Domain.Exceptions;
using Lectio.Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Lectio.Api.Application.Books.TopRatedBooks.Dtos;

namespace Lectio.Api.Application.Books.BookDetails;

public class BookDetailService : IBookDetailService
{
    private readonly AppDbContext _context;

    public BookDetailService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<BookDetailDto> GetBookDetailAsync(int bookId, CancellationToken cancellationToken)
    {
        var book = await _context.Books
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.BookId == bookId, cancellationToken);

        if (book is null)
        {
            throw new NotFoundException(nameof(Book), bookId);
        }

        var authors = await _context.BookAuthors
            .AsNoTracking()
            .Where(bookAuthor => bookAuthor.BookId == bookId)
            .OrderBy(bookAuthor => bookAuthor.AuthorId)
            .Select(bookAuthor => new BookAuthorSummaryDto
            {
                Id = bookAuthor.AuthorId,
                Name = bookAuthor.Author.AuthorName
            })
            .ToListAsync(cancellationToken);

        var categories = await _context.BookCategories
            .AsNoTracking()
            .Where(bookCategory => bookCategory.BookId == bookId)
            .OrderBy(bookCategory => bookCategory.CategoryId)
            .Select(bookCategory => new BookCategorySummaryDto
            {
                Id = bookCategory.CategoryId,
                Name = bookCategory.Category.CategoryName
            })
            .ToListAsync(cancellationToken);

        return new BookDetailDto
        {
            Id = book.BookId,
            Title = book.Title,
            ImageUrl = book.ImageLink,
            Description = book.Description,
            PageCount = book.PageCount,
            Publisher = book.PublisherName,
            PublishedYear = book.PublishedDate?.Year,
            Language = book.Language,
            Authors = authors,
            Categories = categories
        };
    }
}
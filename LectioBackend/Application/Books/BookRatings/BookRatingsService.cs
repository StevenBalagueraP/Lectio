using Lectio.Api.Application.Books.BookRatings.Dtos;
using Lectio.Api.Domain.Entities;
using Lectio.Api.Domain.Exceptions;
using Lectio.Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Lectio.Api.Application.Books.BookRatings;

public class BookRatingsService : IBookRatingsService
{
    private const int MaxPageSize = 50;

    private readonly AppDbContext _context;

    public BookRatingsService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<BookRatingsResponseDto> GetBookRatingsAsync(
        int bookId,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        ValidatePaging(page, pageSize);

        var book = await _context.Books
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.BookId == bookId, cancellationToken);

        if (book is null)
        {
            throw new NotFoundException(nameof(Book), bookId);
        }

        var users = await _context.Ratings
            .AsNoTracking()
            .Where(rating => rating.BookId == bookId)
            .OrderByDescending(rating => rating.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(rating => new BookRatingUserDto
            {
                IdUser = rating.UserId,
                FullName = rating.User.FullName,
                Comment = rating.Comment,
                Rating = rating.Stars, 
                UpdatedAt = rating.UpdatedAt,
                CreatedAt = rating.CreatedAt
            })
            .ToListAsync(cancellationToken);

        return new BookRatingsResponseDto
        {
            AverageRating = book.AverageRating,
            NumberOfRatings = book.RatingCount,
            Users = users
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
}
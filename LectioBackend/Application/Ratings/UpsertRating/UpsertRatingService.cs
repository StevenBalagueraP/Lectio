using Lectio.Api.Application.Ratings.UpsertRating.Dtos;
using Lectio.Api.Domain.Entities;
using Lectio.Api.Domain.Exceptions;
using Lectio.Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Lectio.Api.Application.Ratings.UpsertRating;

public class UpsertRatingService : IUpsertRatingService
{
    private readonly AppDbContext _context;

    public UpsertRatingService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<RatingResponseDto> UpsertRatingAsync(
        int userId,
        UpsertRatingRequestDto request,
        CancellationToken cancellationToken)
    {
        await EnsureUserExistsAsync(userId, cancellationToken);
        await EnsureBookExistsAsync(request.BookId, cancellationToken);

        var latestLoan = await _context.Loans
            .AsNoTracking()
            .Where(loan => loan.BookId == request.BookId && loan.UserId == userId)
            .OrderByDescending(loan => loan.CheckoutDate)
            .FirstOrDefaultAsync(cancellationToken);

        if (latestLoan is null)
        {
            throw new BadRequestException("You must have borrowed this book before rating it.");
        }

        var existingRating = await _context.Ratings
            .FirstOrDefaultAsync(
                rating => rating.UserId == userId && rating.BookId == request.BookId,
                cancellationToken);

        var now = DateTime.UtcNow;
        string message;

        if (existingRating is not null)
        {
            existingRating.Stars = request.Stars;
            existingRating.Comment = request.Comment;
            existingRating.UpdatedAt = now;
            message = "Rating updated successfully.";
        }
        else
        {
            existingRating = new Rating
            {
                BookId = request.BookId,
                UserId = userId,
                LoanId = latestLoan.LoanId,
                Stars = request.Stars,
                Comment = request.Comment,
                CreatedAt = now,
                UpdatedAt = now
            };

            _context.Ratings.Add(existingRating);
            message = "Rating created successfully.";
        }

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException) when (existingRating.RatingId == 0)
        {
            throw new ConflictException("A rating from this user for this book was just created. Please try again.");
        }

        return new RatingResponseDto
        {
            Message = message,
            RatingId = existingRating.RatingId,
            BookId = existingRating.BookId,
            UserId = existingRating.UserId,
            Stars = existingRating.Stars,
            Comment = existingRating.Comment,
            CreatedAt = existingRating.CreatedAt,
            UpdatedAt = existingRating.UpdatedAt
        };
    }

    private async Task EnsureUserExistsAsync(int userId, CancellationToken cancellationToken)
    {
        var userExists = await _context.Users
            .AsNoTracking()
            .AnyAsync(user => user.UserId == userId, cancellationToken);

        if (!userExists)
        {
            throw new NotFoundException(nameof(User), userId);
        }
    }

    private async Task EnsureBookExistsAsync(int bookId, CancellationToken cancellationToken)
    {
        var bookExists = await _context.Books
            .AsNoTracking()
            .AnyAsync(book => book.BookId == bookId, cancellationToken);

        if (!bookExists)
        {
            throw new NotFoundException(nameof(Book), bookId);
        }
    }
}
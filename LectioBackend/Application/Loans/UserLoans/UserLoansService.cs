using Lectio.Api.Application.Books.BrowseBooks.Dtos;
using Lectio.Api.Application.Loans.UserLoans.Dtos;
using Lectio.Api.Domain.Entities;
using Lectio.Api.Domain.Exceptions;
using Lectio.Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Lectio.Api.Application.Loans.UserLoans;

public class UserLoansService : IUserLoansService
{
    private const int MaxPageSize = 50;

    private readonly AppDbContext _context;

    public UserLoansService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<UserLoansResponseDto> GetUserLoansAsync(
    int userId,
    int page,
    int pageSize,
    CancellationToken cancellationToken)
{
    ValidatePaging(page, pageSize);
    await EnsureUserExistsAsync(userId, cancellationToken);

    var query = _context.Loans
        .AsNoTracking()
        .Where(loan => loan.UserId == userId);

    var totalCount = await query.CountAsync(cancellationToken);

    var loans = await query
        .OrderByDescending(loan => loan.CheckoutDate)
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .Select(loan => new
        {
            loan.LoanId,
            loan.CheckoutDate,
            loan.DueDate,
            loan.ReturnDate,
            loan.Book.BookId,
            loan.Book.Title,
            loan.Book.ImageLink,
            loan.Book.AverageRating,
            Authors = _context.BookAuthors
                .Where(bookAuthor => bookAuthor.BookId == loan.Book.BookId)
                .OrderBy(bookAuthor => bookAuthor.AuthorId)
                .Select(bookAuthor => new AuthorSummaryDto
                {
                    Id = bookAuthor.Author.AuthorId,
                    Name = bookAuthor.Author.AuthorName
                })
                .ToList()
        })
        .ToListAsync(cancellationToken);

    var now = DateTime.UtcNow;

    var data = loans
        .Select(loan => new UserLoanDto
        {
            Id = loan.LoanId,
            Book = new LoanBookSummaryDto
            {
                Id = loan.BookId,
                Title = loan.Title,
                ImageUrl = loan.ImageLink,
                AverageRating = loan.AverageRating,
                Authors = loan.Authors
            },
            CheckoutDate = loan.CheckoutDate,
            DueDate = loan.DueDate,
            ReturnDate = loan.ReturnDate,
            Status = ResolveStatus(loan.ReturnDate, loan.DueDate, now)
        })
        .ToList();

    return new UserLoansResponseDto
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

    private static string ResolveStatus(DateTime? returnDate, DateTime dueDate, DateTime now)
    {
        if (returnDate.HasValue)
        {
            return "Returned";
        }

        return now > dueDate ? "Overdue" : "Active";
    }
}
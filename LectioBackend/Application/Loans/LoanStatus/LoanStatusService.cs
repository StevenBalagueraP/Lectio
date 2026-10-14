using Lectio.Api.Application.Loans;
using Lectio.Api.Application.Loans.LoanStatus.Dtos;
using Lectio.Api.Domain.Entities;
using Lectio.Api.Domain.Exceptions;
using Lectio.Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Lectio.Api.Application.Loans.LoanStatus;

public class LoanStatusService : ILoanStatusService
{
    private const string Available = "Available";
    private const string BorrowedByYou = "BorrowedByYou";
    private const string Unavailable = "Unavailable";
    private const string LoginRequired = "LoginRequired";

    private readonly AppDbContext _context;

    public LoanStatusService(AppDbContext context)
    {
        _context = context;
    }


    public async Task<BookLoanStatusResponseDto> GetLoanStatusAsync(
        int? userId,                                      
        int bookId,
        CancellationToken cancellationToken)
    {
        await EnsureBookExistsAsync(bookId, cancellationToken);

        if (userId is null)
        {
            return new BookLoanStatusResponseDto
            {
                BookId = bookId,
                UserId = null,
                Status = LoginRequired,
                LoanId = null,
                DueDate = null
            };
        }

        var currentUserId = userId.Value;
        await EnsureUserExistsAsync(currentUserId, cancellationToken);

        var activeLoan = await _context.Loans
            .AsNoTracking()
            .Where(LoanQueries.IsActiveForBook(bookId))
            .Select(loan => new
            {
                loan.LoanId,
                loan.UserId,
                loan.DueDate
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (activeLoan is null)
        {
            return new BookLoanStatusResponseDto
            {
                BookId = bookId,
                UserId = currentUserId,
                Status = Available,
                LoanId = null,
                DueDate = null
            };
        }

        if (activeLoan.UserId == currentUserId)
        {
            return new BookLoanStatusResponseDto
            {
                BookId = bookId,
                UserId = currentUserId,
                Status = BorrowedByYou,
                LoanId = activeLoan.LoanId,
                DueDate = activeLoan.DueDate
            };
        }

        return new BookLoanStatusResponseDto
        {
            BookId = bookId,
            UserId = currentUserId,
            Status = Unavailable,
            LoanId = null,
            DueDate = null
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

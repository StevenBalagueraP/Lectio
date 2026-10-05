using Lectio.Api.Application.Loans;
using Lectio.Api.Application.Loans.ReturnLoan.Dtos;
using Lectio.Api.Domain.Entities;
using Lectio.Api.Domain.Exceptions;
using Lectio.Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Lectio.Api.Application.Loans.ReturnLoan;

public class ReturnLoanService : IReturnLoanService
{
    private readonly AppDbContext _context;

    public ReturnLoanService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ReturnLoanResponseDto> ReturnLoanAsync(
        int userId,
        int bookId,
        CancellationToken cancellationToken)
    {
        await EnsureUserExistsAsync(userId, cancellationToken);

        var activeLoan = await _context.Loans
            .Include(loan => loan.Book)
            .FirstOrDefaultAsync(LoanQueries.IsActiveForBook(bookId), cancellationToken);

        if (activeLoan is null)
        {
            throw new NotFoundException("This book has no active loan to return (it was never loaned, or it was already returned).");
        }

        if (activeLoan.UserId != userId)
        {
            throw new BadRequestException("This book's active loan does not belong to this user.");
        }

        var returnDate = DateTime.UtcNow;

        activeLoan.ReturnDate = returnDate;
        activeLoan.Book.IsAvailable = true;

        await _context.SaveChangesAsync(cancellationToken);

        return new ReturnLoanResponseDto
        {
            Message = "Book returned successfully.",
            LoanId = activeLoan.LoanId,
            BookId = activeLoan.BookId,
            UserId = activeLoan.UserId,
            ReturnDate = returnDate
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
}
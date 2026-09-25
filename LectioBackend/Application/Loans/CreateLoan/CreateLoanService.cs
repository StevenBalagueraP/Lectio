using Lectio.Api.Application.Loans.CreateLoan.Dtos;
using Lectio.Api.Domain.Entities;
using Lectio.Api.Domain.Exceptions;
using Lectio.Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Lectio.Api.Application.Loans.CreateLoan;

public class CreateLoanService : ICreateLoanService
{
    private readonly AppDbContext _context;

    public CreateLoanService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<CreateLoanResponseDto> CreateLoanAsync(
        int userId,
        CreateLoanRequestDto request,
        CancellationToken cancellationToken)
    {
        ValidateDates(request.CheckoutDate, request.DueDate);

        await EnsureUserExistsAsync(userId, cancellationToken);

        var book = await _context.Books
            .FirstOrDefaultAsync(b => b.BookId == request.BookId, cancellationToken);

        if (book is null)
        {
            throw new NotFoundException(nameof(Book), request.BookId);
        }

        var hasActiveLoan = await _context.Loans
            .AsNoTracking()
            .AnyAsync(loan => loan.BookId == request.BookId && loan.ReturnDate == null, cancellationToken);

        if (hasActiveLoan || !book.IsAvailable)
        {
            throw new ConflictException("This book is currently on loan and is not available until it is returned.");
        }

        var loan = new Loan
        {
            BookId = request.BookId,
            UserId = userId,
            CheckoutDate = request.CheckoutDate,
            DueDate = request.DueDate,
            ReturnDate = null
        };

        book.IsAvailable = false;

        _context.Loans.Add(loan);

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException)
        {
            // A concurrent request won the race for this book's active-loan unique index (IX_Loan_BookId_Active).
            throw new ConflictException("This book is currently on loan and is not available until it is returned.");
        }

        return new CreateLoanResponseDto
        {
            Message = "Book loaned successfully.",
            LoanId = loan.LoanId,
            BookId = loan.BookId,
            UserId = loan.UserId,
            CheckoutDate = loan.CheckoutDate,
            DueDate = loan.DueDate
        };
    }

    private static void ValidateDates(DateTime checkoutDate, DateTime dueDate)
    {
        if (dueDate <= checkoutDate)
        {
            throw new BadRequestException("dueDate must be after checkoutDate.");
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
}
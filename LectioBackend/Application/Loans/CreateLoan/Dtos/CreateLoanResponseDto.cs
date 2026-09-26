namespace Lectio.Api.Application.Loans.CreateLoan.Dtos;

public class CreateLoanResponseDto
{
    public string Message { get; init; } = "Book loaned successfully.";
    public int LoanId { get; init; }
    public int BookId { get; init; }
    public int UserId { get; init; }
    public DateTime CheckoutDate { get; init; }
    public DateTime DueDate { get; init; }
}
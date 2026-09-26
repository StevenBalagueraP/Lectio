namespace Lectio.Api.Application.Loans.ReturnLoan.Dtos;

public class ReturnLoanResponseDto
{
    public string Message { get; init; } = "Book returned successfully.";
    public int LoanId { get; init; }
    public int BookId { get; init; }
    public int UserId { get; init; }
    public DateTime ReturnDate { get; init; }
}
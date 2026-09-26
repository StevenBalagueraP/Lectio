namespace Lectio.Api.Application.Loans.UserLoans.Dtos;

public class UserLoanDto
{
    public int Id { get; init; }
    public LoanBookSummaryDto Book { get; init; } = new();
    public DateTime CheckoutDate { get; init; }
    public DateTime DueDate { get; init; }
    public DateTime? ReturnDate { get; init; }
    public string Status { get; init; } = string.Empty;
}
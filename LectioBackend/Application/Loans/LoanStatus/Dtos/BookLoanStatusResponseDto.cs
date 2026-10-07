namespace Lectio.Api.Application.Loans.LoanStatus.Dtos;

public class BookLoanStatusResponseDto
{
    public int BookId { get; init; }
    public int? UserId { get; init; }
    public string Status { get; init; } = string.Empty;
    public int? LoanId { get; init; }
    public DateTime? DueDate { get; init; }
}

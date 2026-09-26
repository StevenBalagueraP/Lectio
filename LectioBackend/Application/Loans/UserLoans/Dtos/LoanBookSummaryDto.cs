namespace Lectio.Api.Application.Loans.UserLoans.Dtos;

public class LoanBookSummaryDto
{
    public int Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? ImageUrl { get; init; }
    public decimal AverageRating { get; init; }
}
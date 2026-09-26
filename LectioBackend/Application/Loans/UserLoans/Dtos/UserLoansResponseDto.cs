using Lectio.Api.Application.Books.BrowseBooks.Dtos;

namespace Lectio.Api.Application.Loans.UserLoans.Dtos;

public class UserLoansResponseDto
{
    public IReadOnlyList<UserLoanDto> Data { get; init; } = Array.Empty<UserLoanDto>();
    public PaginationDto Pagination { get; init; } = new();
}
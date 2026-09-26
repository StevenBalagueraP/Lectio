using Lectio.Api.Application.Loans.UserLoans.Dtos;

namespace Lectio.Api.Application.Loans.UserLoans;

public interface IUserLoansService
{
    Task<UserLoansResponseDto> GetUserLoansAsync(
        int userId,
        int page,
        int pageSize,
        CancellationToken cancellationToken);
}
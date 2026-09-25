using Lectio.Api.Application.Loans.ReturnLoan.Dtos;

namespace Lectio.Api.Application.Loans.ReturnLoan;

public interface IReturnLoanService
{
    Task<ReturnLoanResponseDto> ReturnLoanAsync(
        int userId,
        int bookId,
        CancellationToken cancellationToken);
}
using Lectio.Api.Application.Loans.LoanStatus.Dtos;

namespace Lectio.Api.Application.Loans.LoanStatus;

public interface ILoanStatusService
{
    Task<BookLoanStatusResponseDto> GetLoanStatusAsync(
        int userId,
        int bookId,
        CancellationToken cancellationToken);
}

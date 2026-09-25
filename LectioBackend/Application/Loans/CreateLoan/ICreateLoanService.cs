using Lectio.Api.Application.Loans.CreateLoan.Dtos;

namespace Lectio.Api.Application.Loans.CreateLoan;

public interface ICreateLoanService
{
    Task<CreateLoanResponseDto> CreateLoanAsync(
        int userId,
        CreateLoanRequestDto request,
        CancellationToken cancellationToken);
}
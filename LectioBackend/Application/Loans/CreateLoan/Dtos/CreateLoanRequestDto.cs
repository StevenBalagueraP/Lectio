using System.ComponentModel.DataAnnotations;

namespace Lectio.Api.Application.Loans.CreateLoan.Dtos;

public class CreateLoanRequestDto
{
    [Required]
    public int BookId { get; init; }

    [Required]
    public DateTime CheckoutDate { get; init; }

    [Required]
    public DateTime DueDate { get; init; }
}
using System.ComponentModel.DataAnnotations;

namespace Lectio.Api.Application.Auth.Dtos;

public class RegisterRequestDto
{
    [Required]
    [StringLength(255, MinimumLength = 2)]
    public string FullName { get; init; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(255)]
    public string Email { get; init; } = string.Empty;

    [Required]
    [StringLength(72, MinimumLength = 8)]
    [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d).+$", ErrorMessage = "Password must contain at least one letter and one number.")]
    public string Password { get; init; } = string.Empty;

    [Phone]
    [StringLength(20)]
    public string? Phone { get; init; }
}

using System.ComponentModel.DataAnnotations;

namespace Lectio.Api.Application.Auth.Dtos;

public class RefreshTokenRequestDto
{
    [Required]
    public string RefreshToken { get; init; } = string.Empty;
}

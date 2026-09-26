namespace Lectio.Api.Application.Auth.Dtos;

public class AuthResponseDto
{
    public string AccessToken { get; init; } = string.Empty;
    public string RefreshToken { get; init; } = string.Empty;
    public string TokenType { get; init; } = "Bearer";

    public int ExpiresIn { get; init; }

    public AuthUserDto User { get; init; } = new();
}

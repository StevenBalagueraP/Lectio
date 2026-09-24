namespace Lectio.Api.Application.Auth.Dtos;

public class AuthUserDto
{
    public int Id { get; init; }
    public string FullName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string? Phone { get; init; }
    public string Role { get; init; } = string.Empty;
}

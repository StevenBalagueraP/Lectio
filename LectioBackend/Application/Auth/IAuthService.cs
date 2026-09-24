using Lectio.Api.Application.Auth.Dtos;

namespace Lectio.Api.Application.Auth;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request, CancellationToken cancellationToken);
    Task<AuthResponseDto> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken);
    Task<AuthResponseDto> RefreshAsync(string refreshToken, CancellationToken cancellationToken);
    Task LogoutAsync(string refreshToken, CancellationToken cancellationToken);
}

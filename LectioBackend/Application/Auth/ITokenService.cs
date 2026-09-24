using Lectio.Api.Domain.Entities;

namespace Lectio.Api.Application.Auth;

public interface ITokenService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
    string HashToken(string token);
}

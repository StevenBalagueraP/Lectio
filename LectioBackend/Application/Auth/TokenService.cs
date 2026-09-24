using System.Security.Cryptography;
using System.Text;
using Lectio.Api.Domain.Entities;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace Lectio.Api.Application.Auth;

public class TokenService : ITokenService
{
    private readonly JwtSettings _settings;
    private readonly SigningCredentials _signingCredentials;
    private readonly JsonWebTokenHandler _tokenHandler = new();

    public TokenService(IOptions<JwtSettings> options)
    {
        _settings = options.Value;

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Secret));
        _signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
    }

    public string GenerateAccessToken(User user)
    {
        var claims = new Dictionary<string, object>
        {
            [JwtRegisteredClaimNames.Sub] = user.UserId.ToString(),
            [JwtRegisteredClaimNames.Email] = user.Email,
            [JwtRegisteredClaimNames.Jti] = Guid.NewGuid().ToString(),
            ["fullName"] = user.FullName,
            ["role"] = user.Role
        };

        if (!string.IsNullOrWhiteSpace(user.Phone))
        {
            claims["phone"] = user.Phone;
        }

        var now = DateTime.UtcNow;

        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = _settings.Issuer,
            Audience = _settings.Audience,
            Claims = claims,
            IssuedAt = now,
            NotBefore = now,
            Expires = now.AddMinutes(_settings.AccessTokenMinutes),
            SigningCredentials = _signingCredentials
        };

        return _tokenHandler.CreateToken(descriptor);
    }

    public string GenerateRefreshToken()
        => WebEncoders.Base64UrlEncode(RandomNumberGenerator.GetBytes(64));

    public string HashToken(string token)
        => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
}

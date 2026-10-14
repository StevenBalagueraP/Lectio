using System.Text;
using Lectio.Api.Application.Auth.Dtos;
using Lectio.Api.Domain.Entities;
using Lectio.Api.Domain.Exceptions;
using Lectio.Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Npgsql;

namespace Lectio.Api.Application.Auth;

public class AuthService : IAuthService
{
    private const int MaxPasswordBytes = 72;

    private static readonly string DummyPasswordHash = BCrypt.Net.BCrypt.HashPassword(Guid.NewGuid().ToString());

    private readonly AppDbContext _context;
    private readonly ITokenService _tokenService;
    private readonly JwtSettings _jwtSettings;

    public AuthService(AppDbContext context, ITokenService tokenService, IOptions<JwtSettings> jwtOptions)
    {
        _context = context;
        _tokenService = tokenService;
        _jwtSettings = jwtOptions.Value;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request, CancellationToken cancellationToken)
    {
        if (Encoding.UTF8.GetByteCount(request.Password) > MaxPasswordBytes)
        {
            throw new BadRequestException($"Password must not exceed {MaxPasswordBytes} bytes.");
        }

        var email = NormalizeEmail(request.Email);

        var emailTaken = await _context.Users.AnyAsync(u => u.Email == email, cancellationToken);
        if (emailTaken)
        {
            throw new ConflictException("Email is already registered.");
        }

        var user = new User
        {
            FullName = request.FullName.Trim(),
            Email = email,
            Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Phone = string.IsNullOrWhiteSpace(request.Phone) ? null : request.Phone.Trim()
        };

        _context.Users.Add(user);

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
        {
            throw new ConflictException("Email is already registered.");
        }

        return await IssueTokensAsync(user, cancellationToken);
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken)
    {
        var email = NormalizeEmail(request.Email);

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

        var passwordIsValid = BCrypt.Net.BCrypt.Verify(request.Password, user?.Password ?? DummyPasswordHash);

        if (user is null || !passwordIsValid)
        {
            throw new UnauthorizedException("Invalid email or password.");
        }

        return await IssueTokensAsync(user, cancellationToken);
    }

    public async Task<AuthResponseDto> RefreshAsync(string refreshToken, CancellationToken cancellationToken)
    {
        var tokenHash = _tokenService.HashToken(refreshToken);

        var stored = await _context.RefreshTokens
            .AsNoTracking()
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.TokenHash == tokenHash, cancellationToken);

        if (stored is null)
        {
            throw new UnauthorizedException("Invalid refresh token.");
        }

        if (stored.RevokedAt is not null)
        {
            if (stored.ReplacedByTokenHash is not null)
            {
                await RevokeAllForUserAsync(stored.UserId, cancellationToken);
            }

            throw new UnauthorizedException("Refresh token is no longer valid.");
        }

        if (stored.IsExpired)
        {
            throw new UnauthorizedException("Refresh token has expired.");
        }

        var newRefreshToken = _tokenService.GenerateRefreshToken();
        var newTokenHash = _tokenService.HashToken(newRefreshToken);
        var now = DateTime.UtcNow;

        var rotated = await _context.RefreshTokens
            .Where(t => t.RefreshTokenId == stored.RefreshTokenId && t.RevokedAt == null)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(t => t.RevokedAt, now)
                .SetProperty(t => t.ReplacedByTokenHash, newTokenHash),
                cancellationToken);

        if (rotated == 0)
        {
            await RevokeAllForUserAsync(stored.UserId, cancellationToken);
            throw new UnauthorizedException("Refresh token is no longer valid.");
        }

        _context.RefreshTokens.Add(new RefreshToken
        {
            UserId = stored.UserId,
            TokenHash = newTokenHash,
            CreatedAt = now,
            ExpiresAt = now.AddDays(_jwtSettings.RefreshTokenDays)
        });

        await _context.SaveChangesAsync(cancellationToken);

        return BuildResponse(stored.User, newRefreshToken);
    }

    public async Task LogoutAsync(string refreshToken, CancellationToken cancellationToken)
    {
        var tokenHash = _tokenService.HashToken(refreshToken);
        var now = DateTime.UtcNow;

        await _context.RefreshTokens
            .Where(t => t.TokenHash == tokenHash && t.RevokedAt == null)
            .ExecuteUpdateAsync(setters => setters.SetProperty(t => t.RevokedAt, now), cancellationToken);
    }

    private async Task<AuthResponseDto> IssueTokensAsync(User user, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;

        await _context.RefreshTokens
            .Where(t => t.UserId == user.UserId && t.ExpiresAt < now)
            .ExecuteDeleteAsync(cancellationToken);

        var refreshToken = _tokenService.GenerateRefreshToken();

        _context.RefreshTokens.Add(new RefreshToken
        {
            UserId = user.UserId,
            TokenHash = _tokenService.HashToken(refreshToken),
            CreatedAt = now,
            ExpiresAt = now.AddDays(_jwtSettings.RefreshTokenDays)
        });

        await _context.SaveChangesAsync(cancellationToken);

        return BuildResponse(user, refreshToken);
    }

    private AuthResponseDto BuildResponse(User user, string refreshToken) => new()
    {
        AccessToken = _tokenService.GenerateAccessToken(user),
        RefreshToken = refreshToken,
        ExpiresIn = _jwtSettings.AccessTokenMinutes * 60,
        User = new AuthUserDto
        {
            Id = user.UserId,
            FullName = user.FullName,
            Email = user.Email,
            Phone = user.Phone,
            Role = user.Role
        }
    };

    private Task<int> RevokeAllForUserAsync(int userId, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;

        return _context.RefreshTokens
            .Where(t => t.UserId == userId && t.RevokedAt == null)
            .ExecuteUpdateAsync(setters => setters.SetProperty(t => t.RevokedAt, now), cancellationToken);
    }

    private static string NormalizeEmail(string email) => email.Trim().ToLowerInvariant();
}

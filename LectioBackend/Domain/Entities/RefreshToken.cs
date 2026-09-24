namespace Lectio.Api.Domain.Entities;

public class RefreshToken
{
    public int RefreshTokenId { get; set; }

    public int UserId { get; set; }
    public User User { get; set; } = null!;

    // SHA-256 (hex) of the token. The raw token is never stored.
    public string TokenHash { get; set; } = null!;

    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime? RevokedAt { get; set; }

    // Set when the token was rotated (used to get a new one). Lets us detect reuse.
    public string? ReplacedByTokenHash { get; set; }

    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
}

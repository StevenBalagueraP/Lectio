using Lectio.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lectio.Api.Infrastructure.Data.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.HasKey(t => t.RefreshTokenId);

        builder.Property(t => t.TokenHash)
            .IsRequired()
            .HasMaxLength(64);

        builder.Property(t => t.ReplacedByTokenHash)
            .HasMaxLength(64);

        builder.Property(t => t.CreatedAt)
            .HasDefaultValueSql("now()");

        builder.HasIndex(t => t.TokenHash)
            .IsUnique()
            .HasDatabaseName("IX_RefreshToken_TokenHash");

        builder.HasIndex(t => new { t.UserId, t.RevokedAt })
            .HasDatabaseName("IX_RefreshToken_UserId_RevokedAt");

        builder.HasOne(t => t.User)
            .WithMany()
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

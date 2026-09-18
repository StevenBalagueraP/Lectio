using Lectio.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lectio.Api.Infrastructure.Data.Configurations;

public class RatingConfiguration : IEntityTypeConfiguration<Rating>
{
    public void Configure(EntityTypeBuilder<Rating> builder)
    {
        builder.HasKey(r => r.RatingId);

        builder.Property(r => r.Comment)
            .HasMaxLength(1000);

        builder.Property(r => r.CreatedAt)
            .HasDefaultValueSql("now()");

        builder.Property(r => r.UpdatedAt)
            .HasDefaultValueSql("now()");

        builder.HasIndex(r => new { r.UserId, r.BookId })
            .IsUnique()
            .HasDatabaseName("IX_Rating_UserId_BookId");

        builder.HasIndex(r => new { r.BookId, r.CreatedAt })
            .IsDescending(false, true)
            .HasDatabaseName("IX_Rating_BookId_CreatedAt");

        builder.HasOne(r => r.Book)
            .WithMany()
            .HasForeignKey(r => r.BookId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.User)
            .WithMany()
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Loan)
            .WithMany()
            .HasForeignKey(r => r.LoanId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
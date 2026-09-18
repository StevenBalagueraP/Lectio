using Lectio.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lectio.Api.Infrastructure.Data.Configurations;

public class BookConfiguration : IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Book> builder)
    {
        builder.HasKey(b => b.BookId);

        builder.Property(b => b.Title)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(b => b.PublisherName)
            .HasMaxLength(255);

        builder.Property(b => b.ImageLink)
            .HasMaxLength(1000);

        builder.Property(b => b.Language)
            .HasMaxLength(50);

        builder.Property(b => b.AverageRating)
            .HasColumnType("numeric(3,2)")
            .HasDefaultValue(0);

        builder.Property(b => b.RatingCount)
            .HasDefaultValue(0);

        builder.Property(b => b.Isbn)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(b => b.IsAvailable)
            .HasDefaultValue(true);

        builder.HasIndex(b => b.Title)
            .HasDatabaseName("IX_Book_Title");

        builder.HasIndex(b => b.PublishedDate)
            .HasDatabaseName("IX_Book_PublishedDate");

        builder.HasIndex(b => b.AverageRating)
            .IsDescending()
            .HasDatabaseName("IX_Book_AverageRating");

        builder.HasIndex(b => b.Isbn)
            .IsUnique()
            .HasDatabaseName("IX_Book_Isbn");
    }
}
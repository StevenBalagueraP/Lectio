using Lectio.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lectio.Api.Infrastructure.Data.Configurations;

public class BookCategoryConfiguration : IEntityTypeConfiguration<BookCategory>
{
    public void Configure(EntityTypeBuilder<BookCategory> builder)
    {
        builder.HasKey(ba => new { ba.BookId, ba.CategoryId });

        builder.HasIndex(ba => ba.CategoryId)
            .HasDatabaseName("IX_BookCategory_CategoryId");

        builder.HasOne(ba => ba.Book)
            .WithMany()
            .HasForeignKey(ba => ba.BookId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ba => ba.Category)
            .WithMany()
            .HasForeignKey(ba => ba.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

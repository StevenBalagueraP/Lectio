using Lectio.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lectio.Api.Infrastructure.Data.Configurations;

public class LoanConfiguration : IEntityTypeConfiguration<Loan>
{
    public void Configure(EntityTypeBuilder<Loan> builder)
    {
        builder.HasKey(l => l.LoanId);

        builder.HasIndex(l => l.DueDate)
            .HasDatabaseName("IX_Loan_DueDate");

        builder.HasIndex(l => l.BookId)
            .IsUnique()
            .HasFilter("\"ReturnDate\" IS NULL")
            .HasDatabaseName("IX_Loan_BookId_Active");

        builder.HasIndex(l => new { l.UserId, l.ReturnDate })
            .HasDatabaseName("IX_Loan_UserId_ReturnDate");

        builder.HasOne(l => l.Book)
            .WithMany()
            .HasForeignKey(l => l.BookId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(l => l.User)
            .WithMany()
            .HasForeignKey(l => l.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
using Lectio.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lectio.Api.Infrastructure.Data.Configurations;

public class AuthorConfiguration : IEntityTypeConfiguration<Author>
{
    public void Configure(EntityTypeBuilder<Author> builder)
    {
        builder.HasKey(b => b.AuthorId);

        builder.Property(b => b.AuthorName)
            .IsRequired()
            .HasMaxLength(255);
    }
}
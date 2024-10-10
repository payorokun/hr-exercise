using Crud.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Crud.Infrastructure.Data;
internal class QuoteConfiguration : IEntityTypeConfiguration<Quote>
{
    public void Configure(EntityTypeBuilder<Quote> builder)
    {
        builder.HasKey(q => q.Id);
        builder.Property(q => q.Author).IsRequired().HasMaxLength(50);
        builder.Property(q => q.Text).IsRequired().HasMaxLength(500);
        builder.Property(q => q.TextLength).IsRequired().HasMaxLength(500);
    }
}

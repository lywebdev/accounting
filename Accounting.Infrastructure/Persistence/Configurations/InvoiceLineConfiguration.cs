using Accounting.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Accounting.Infrastructure.Persistence.Configurations;

public class InvoiceLineConfiguration : IEntityTypeConfiguration<InvoiceLine>
{
    public void Configure(EntityTypeBuilder<InvoiceLine> builder)
    {
        builder.ToTable("InvoiceLines");
        builder.HasKey(l => l.Id);
        builder.Property(l => l.Description).HasMaxLength(256);
        builder.Property(l => l.VatRate).HasColumnType("decimal(5,2)");
        var priceBuilder = builder.OwnsOne(l => l.UnitPrice);
        priceBuilder.Property(p => p.Amount).HasColumnType("decimal(18,2)");
        priceBuilder.Property(p => p.Currency).HasMaxLength(3).IsRequired();
    }
}

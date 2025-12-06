using Accounting.Core.Entities;
using Accounting.Core.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Accounting.Infrastructure.Persistence.Configurations;

public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        builder.ToTable("Invoices");
        builder.HasKey(i => i.Id);
        builder.Property(i => i.Number).HasMaxLength(32).IsRequired();
        builder.Property(i => i.Counterparty).HasMaxLength(128).IsRequired();
        builder.Property(i => i.Type).HasConversion<int>();
        builder.Property(i => i.WorkflowStatus).HasConversion<int>().HasDefaultValue(InvoiceWorkflowStatus.Draft);
        builder.Property(i => i.SentAt);
        builder.Property(i => i.PaidAt);

        builder.HasMany(i => i.Lines)
            .WithOne()
            .HasForeignKey(l => l.InvoiceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(i => i.Lines).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}

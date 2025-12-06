using Accounting.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Accounting.Infrastructure.Persistence.Configurations;

public class BankTransactionConfiguration : IEntityTypeConfiguration<BankTransaction>
{
    public void Configure(EntityTypeBuilder<BankTransaction> builder)
    {
        builder.ToTable("BankTransactions");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Counterparty).HasMaxLength(128);
        builder.Property(t => t.Reference).HasMaxLength(128);
        builder.Property(t => t.BookingDate).HasColumnType("date");
        var amountBuilder = builder.OwnsOne(t => t.Amount);
        amountBuilder.Property(m => m.Amount).HasColumnType("decimal(18,2)");
        amountBuilder.Property(m => m.Currency).HasMaxLength(3).IsRequired();
    }
}

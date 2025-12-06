using Accounting.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Accounting.Infrastructure.Persistence.Configurations;

public class JournalEntryConfiguration : IEntityTypeConfiguration<JournalEntry>
{
    public void Configure(EntityTypeBuilder<JournalEntry> builder)
    {
        builder.ToTable("JournalEntries");
        builder.HasKey(j => j.Id);
        builder.Property(j => j.Reference).HasMaxLength(64).IsRequired();
        builder.Property(j => j.Memo).HasMaxLength(256);
        builder.Property(j => j.EntryDate).HasColumnType("date");

        builder.HasMany(j => j.Lines)
            .WithOne()
            .HasForeignKey(l => l.JournalEntryId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(j => j.Lines).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}

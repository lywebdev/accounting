using Accounting.Core.Entities;
using Accounting.Core.Interfaces.Repositories;
using Accounting.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Accounting.Infrastructure.Repositories;

public class JournalEntryRepository(AccountingDbContext dbContext) : IJournalEntryRepository
{
    public async Task<JournalEntry?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await dbContext.JournalEntries
            .Include("_lines")
            .FirstOrDefaultAsync(j => j.Id == id, cancellationToken);

    public async Task<IReadOnlyList<JournalEntry>> GetAsync(DateOnly? from, DateOnly? to, CancellationToken cancellationToken = default)
    {
        var query = dbContext.JournalEntries.AsQueryable();
        if (from.HasValue)
        {
            query = query.Where(j => j.EntryDate >= from);
        }

        if (to.HasValue)
        {
            query = query.Where(j => j.EntryDate <= to);
        }

        return await query.OrderByDescending(j => j.EntryDate)
            .Include("_lines")
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(JournalEntry entry, CancellationToken cancellationToken = default)
    {
        await dbContext.JournalEntries.AddAsync(entry, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(JournalEntry entry, CancellationToken cancellationToken = default)
    {
        dbContext.JournalEntries.Update(entry);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await dbContext.JournalEntries.Where(j => j.Id == id).ExecuteDeleteAsync(cancellationToken);
    }
}

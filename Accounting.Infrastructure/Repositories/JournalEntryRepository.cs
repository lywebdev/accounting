using Accounting.Core.Entities;
using Accounting.Core.Interfaces.Repositories;
using Accounting.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Accounting.Infrastructure.Repositories;

public class JournalEntryRepository(AccountingDbContext dbContext) : RepositoryBase<JournalEntry>(dbContext), IJournalEntryRepository
{
    public Task<JournalEntry?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => Query()
            .Include(j => j.Lines)
            .FirstOrDefaultAsync(j => j.Id == id, cancellationToken);

    public async Task<IReadOnlyList<JournalEntry>> GetAsync(DateOnly? from, DateOnly? to, CancellationToken cancellationToken = default)
    {
        IQueryable<JournalEntry> query = Query().Include(j => j.Lines);
        if (from.HasValue)
        {
            query = query.Where(j => j.EntryDate >= from);
        }

        if (to.HasValue)
        {
            query = query.Where(j => j.EntryDate <= to);
        }

        query = query.OrderByDescending(j => j.EntryDate);
        return await ToListAsync(query, cancellationToken);
    }

    public Task AddAsync(JournalEntry entry, CancellationToken cancellationToken = default)
        => AddEntityAsync(entry, cancellationToken);

    public Task UpdateAsync(JournalEntry entry, CancellationToken cancellationToken = default)
        => UpdateEntityAsync(entry, cancellationToken);

    public Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        => DeleteWhereAsync(j => j.Id == id, cancellationToken);
}

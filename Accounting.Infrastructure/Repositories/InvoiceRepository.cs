using Accounting.Core.Entities;
using Accounting.Core.Enums;
using Accounting.Core.Interfaces.Repositories;
using Accounting.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Accounting.Infrastructure.Repositories;

public class InvoiceRepository(AccountingDbContext dbContext) : RepositoryBase<Invoice>(dbContext), IInvoiceRepository
{
    public Task<Invoice?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => Query()
            .Include(i => i.Lines)
            .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Invoice>> GetAsync(InvoiceType? type, DateOnly? from, DateOnly? to, string? searchTerm, CancellationToken cancellationToken = default)
    {
        IQueryable<Invoice> query = Query().Include(i => i.Lines);

        if (type.HasValue)
        {
            query = query.Where(i => i.Type == type);
        }

        if (from.HasValue)
        {
            query = query.Where(i => i.IssueDate >= from);
        }

        if (to.HasValue)
        {
            query = query.Where(i => i.IssueDate <= to);
        }

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim();
            query = query.Where(i =>
                EF.Functions.Like(i.Number, $"%{term}%") ||
                EF.Functions.Like(i.Counterparty, $"%{term}%"));
        }

        query = query.OrderByDescending(i => i.IssueDate);
        return await ToListAsync(query, cancellationToken);
    }

    public Task AddAsync(Invoice invoice, CancellationToken cancellationToken = default)
        => AddEntityAsync(invoice, cancellationToken);

    public Task UpdateAsync(Invoice invoice, CancellationToken cancellationToken = default)
        => UpdateEntityAsync(invoice, cancellationToken);

    public Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        => DeleteWhereAsync(i => i.Id == id, cancellationToken);
}

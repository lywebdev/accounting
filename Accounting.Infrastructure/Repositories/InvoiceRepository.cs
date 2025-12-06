using Accounting.Core.Entities;
using Accounting.Core.Enums;
using Accounting.Core.Interfaces.Repositories;
using Accounting.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Accounting.Infrastructure.Repositories;

public class InvoiceRepository(AccountingDbContext dbContext) : IInvoiceRepository
{
    public async Task<Invoice?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await dbContext.Invoices
            .Include(i => i.Lines)
            .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Invoice>> GetAsync(InvoiceType? type, DateOnly? from, DateOnly? to, CancellationToken cancellationToken = default)
    {
        var query = dbContext.Invoices.AsQueryable();
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

        return await query.OrderByDescending(i => i.IssueDate)
            .Include(i => i.Lines)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Invoice invoice, CancellationToken cancellationToken = default)
    {
        await dbContext.Invoices.AddAsync(invoice, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Invoice invoice, CancellationToken cancellationToken = default)
    {
        dbContext.Invoices.Update(invoice);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await dbContext.Invoices.Where(i => i.Id == id).ExecuteDeleteAsync(cancellationToken);
    }
}

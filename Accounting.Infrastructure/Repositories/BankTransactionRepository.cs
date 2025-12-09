using Accounting.Core.Entities;
using Accounting.Core.Interfaces.Repositories;
using Accounting.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Accounting.Infrastructure.Repositories;

public class BankTransactionRepository(AccountingDbContext dbContext) : IBankTransactionRepository
{
    public Task<BankTransaction?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => dbContext.BankTransactions.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

    public async Task<IReadOnlyList<BankTransaction>> GetAsync(
        DateOnly? from,
        DateOnly? to,
        bool? isMatched,
        string? searchTerm,
        decimal? amountMin,
        decimal? amountMax,
        CancellationToken cancellationToken = default)
    {
        var query = dbContext.BankTransactions.AsQueryable();

        if (from.HasValue)
        {
            query = query.Where(t => t.BookingDate >= from);
        }

        if (to.HasValue)
        {
            query = query.Where(t => t.BookingDate <= to);
        }

        if (isMatched.HasValue)
        {
            query = isMatched.Value
                ? query.Where(t => t.MatchedInvoiceId != null)
                : query.Where(t => t.MatchedInvoiceId == null);
        }

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim();
            query = query.Where(t =>
                EF.Functions.Like(t.Counterparty, $"%{term}%") ||
                EF.Functions.Like(t.Reference, $"%{term}%"));
        }

        if (amountMin.HasValue)
        {
            var min = amountMin.Value;
            query = query.Where(t => t.Amount.Amount >= min);
        }

        if (amountMax.HasValue)
        {
            var max = amountMax.Value;
            query = query.Where(t => t.Amount.Amount <= max);
        }

        return await query.AsNoTracking()
            .OrderByDescending(t => t.BookingDate)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(BankTransaction transaction, CancellationToken cancellationToken = default)
    {
        await dbContext.BankTransactions.AddAsync(transaction, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(BankTransaction transaction, CancellationToken cancellationToken = default)
    {
        dbContext.BankTransactions.Update(transaction);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}

using Accounting.Core.Entities;
using Accounting.Core.Enums;
using Accounting.Core.Interfaces.Repositories;
using Accounting.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Accounting.Infrastructure.Repositories;

public class AccountRepository(AccountingDbContext dbContext) : IAccountRepository
{
    public async Task<Account?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await dbContext.Accounts.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

    public async Task<Account?> GetByNumberAsync(string number, CancellationToken cancellationToken = default)
        => await dbContext.Accounts.AsNoTracking().FirstOrDefaultAsync(a => a.Number == number, cancellationToken);

    public async Task<IReadOnlyList<Account>> GetAsync(AccountCategory? category, CancellationToken cancellationToken = default)
    {
        var query = dbContext.Accounts.AsQueryable();
        if (category.HasValue)
        {
            query = query.Where(a => a.Category == category);
        }

        return await query.AsNoTracking().OrderBy(a => a.Number).ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Account account, CancellationToken cancellationToken = default)
    {
        await dbContext.Accounts.AddAsync(account, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Account account, CancellationToken cancellationToken = default)
    {
        dbContext.Accounts.Update(account);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await dbContext.Accounts.Where(a => a.Id == id).ExecuteDeleteAsync(cancellationToken);
    }
}

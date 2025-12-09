using Accounting.Core.Entities;
using Accounting.Core.Enums;
using Accounting.Core.Interfaces.Repositories;
using Accounting.Infrastructure.Persistence;

namespace Accounting.Infrastructure.Repositories;

public class AccountRepository(AccountingDbContext dbContext) : RepositoryBase<Account>(dbContext), IAccountRepository
{
    public Task<Account?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

    public Task<Account?> GetByNumberAsync(string number, CancellationToken cancellationToken = default)
        => FirstOrDefaultAsync(a => a.Number == number, cancellationToken);

    public async Task<IReadOnlyList<Account>> GetAsync(AccountCategory? category, CancellationToken cancellationToken = default)
    {
        IQueryable<Account> query = Query();
        if (category.HasValue)
        {
            query = query.Where(a => a.Category == category);
        }

        query = query.OrderBy(a => a.Number);
        return await ToListAsync(query, cancellationToken);
    }

    public Task AddAsync(Account account, CancellationToken cancellationToken = default)
        => AddEntityAsync(account, cancellationToken);

    public Task UpdateAsync(Account account, CancellationToken cancellationToken = default)
        => UpdateEntityAsync(account, cancellationToken);

    public Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        => DeleteWhereAsync(a => a.Id == id, cancellationToken);
}

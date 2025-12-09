using Accounting.Core.Entities;
using Accounting.Core.Interfaces.Repositories;
using Accounting.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Accounting.Infrastructure.Repositories;

public class TaxDeclarationRepository(AccountingDbContext dbContext) : RepositoryBase<TaxDeclaration>(dbContext), ITaxDeclarationRepository
{
    public Task<TaxDeclaration?> GetByPeriodAsync(int year, int period, CancellationToken cancellationToken = default)
        => Query(asNoTracking: false).FirstOrDefaultAsync(t => t.Year == year && t.Period == period, cancellationToken);

    public async Task<IReadOnlyList<TaxDeclaration>> GetByYearAsync(int year, CancellationToken cancellationToken = default)
    {
        var query = Query().Where(t => t.Year == year).OrderBy(t => t.Period);
        return await ToListAsync(query, cancellationToken);
    }

    public async Task SaveAsync(TaxDeclaration declaration, CancellationToken cancellationToken = default)
    {
        if (DbContext.Entry(declaration).State == EntityState.Detached)
        {
            await Set.AddAsync(declaration, cancellationToken);
        }

        await DbContext.SaveChangesAsync(cancellationToken);
    }
}

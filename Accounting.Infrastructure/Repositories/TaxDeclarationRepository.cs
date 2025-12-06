using Accounting.Core.Entities;
using Accounting.Core.Interfaces.Repositories;
using Accounting.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Accounting.Infrastructure.Repositories;

public class TaxDeclarationRepository(AccountingDbContext dbContext) : ITaxDeclarationRepository
{
    public async Task<TaxDeclaration?> GetByPeriodAsync(int year, int period, CancellationToken cancellationToken = default)
        => await dbContext.TaxDeclarations.FirstOrDefaultAsync(t => t.Year == year && t.Period == period, cancellationToken);

    public async Task<IReadOnlyList<TaxDeclaration>> GetByYearAsync(int year, CancellationToken cancellationToken = default)
        => await dbContext.TaxDeclarations.AsNoTracking().Where(t => t.Year == year).OrderBy(t => t.Period).ToListAsync(cancellationToken);

    public async Task SaveAsync(TaxDeclaration declaration, CancellationToken cancellationToken = default)
    {
        if (dbContext.Entry(declaration).State == EntityState.Detached)
        {
            await dbContext.TaxDeclarations.AddAsync(declaration, cancellationToken);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}

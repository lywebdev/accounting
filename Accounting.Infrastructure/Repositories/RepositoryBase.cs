using System.Linq.Expressions;
using Accounting.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Accounting.Infrastructure.Repositories;

public abstract class RepositoryBase<TEntity> where TEntity : class
{
    protected RepositoryBase(AccountingDbContext dbContext)
    {
        DbContext = dbContext;
        Set = dbContext.Set<TEntity>();
    }

    protected AccountingDbContext DbContext { get; }
    protected DbSet<TEntity> Set { get; }

    protected IQueryable<TEntity> Query(bool asNoTracking = true)
        => asNoTracking ? Set.AsNoTracking() : Set.AsQueryable();

    protected Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken)
        => Query().FirstOrDefaultAsync(predicate, cancellationToken);

    protected Task<List<TEntity>> ToListAsync(IQueryable<TEntity> query, CancellationToken cancellationToken)
        => query.AsNoTracking().ToListAsync(cancellationToken);

    protected async Task AddEntityAsync(TEntity entity, CancellationToken cancellationToken)
    {
        await Set.AddAsync(entity, cancellationToken);
        await DbContext.SaveChangesAsync(cancellationToken);
    }

    protected async Task UpdateEntityAsync(TEntity entity, CancellationToken cancellationToken)
    {
        DbContext.ChangeTracker.Clear();
        DbContext.Attach(entity);
        DbContext.Entry(entity).State = EntityState.Modified;
        await DbContext.SaveChangesAsync(cancellationToken);
    }

    protected Task DeleteWhereAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken)
        => Set.Where(predicate).ExecuteDeleteAsync(cancellationToken);
}

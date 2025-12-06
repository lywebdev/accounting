using Accounting.Core.Entities;

namespace Accounting.Core.Interfaces.Repositories;

public interface ITaxDeclarationRepository
{
    Task<TaxDeclaration?> GetByPeriodAsync(int year, int period, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TaxDeclaration>> GetByYearAsync(int year, CancellationToken cancellationToken = default);
    Task SaveAsync(TaxDeclaration declaration, CancellationToken cancellationToken = default);
}

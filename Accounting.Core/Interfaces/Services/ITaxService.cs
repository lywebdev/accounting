using Accounting.Core.Entities;

namespace Accounting.Core.Interfaces.Services;

public interface ITaxService
{
    Task<TaxDeclaration> CalculateAsync(int year, int period, CancellationToken cancellationToken = default);
    Task<TaxDeclaration> SubmitAsync(int year, int period, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TaxDeclaration>> GetYearAsync(int year, CancellationToken cancellationToken = default);
    Task<bool> ValidateVatNumberAsync(string vatNumber, CancellationToken cancellationToken = default);
}

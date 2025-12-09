using Accounting.Core.Entities;
using Accounting.Core.Enums;

namespace Accounting.Core.Interfaces.Services;

public interface IInvoiceQueryService
{
    Task<IReadOnlyList<Invoice>> GetAsync(InvoiceType? type, DateOnly? from, DateOnly? to, string? searchTerm, CancellationToken cancellationToken = default);
    Task<Invoice> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}

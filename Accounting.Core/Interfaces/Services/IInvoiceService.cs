using Accounting.Core.Entities;
using Accounting.Core.Enums;

namespace Accounting.Core.Interfaces.Services;

public interface IInvoiceService
{
    Task<Invoice> CreateAsync(InvoiceType type, string number, string counterparty, DateOnly issueDate, DateOnly dueDate, IEnumerable<InvoiceLine> lines, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Invoice>> GetAsync(InvoiceType? type, DateOnly? from, DateOnly? to, CancellationToken cancellationToken = default);
    Task<Invoice> PostAsync(Guid id, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

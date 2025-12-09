using Accounting.Core.Entities;
using Accounting.Core.Enums;

namespace Accounting.Core.Interfaces.Services;

public interface IInvoiceCommandService
{
    Task<Invoice> CreateAsync(InvoiceType type, string number, string counterparty, DateOnly issueDate, DateOnly dueDate, IEnumerable<InvoiceLine> lines, CancellationToken cancellationToken = default);
    Task<Invoice> PostAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Invoice> MarkSentAsync(Guid id, DateOnly sentDate, CancellationToken cancellationToken = default);
    Task<Invoice> RecordPaymentAsync(Guid id, DateOnly paymentDate, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

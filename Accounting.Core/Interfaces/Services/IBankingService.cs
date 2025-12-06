using Accounting.Core.Entities;
using Accounting.Core.Models.Banking;

namespace Accounting.Core.Interfaces.Services;

public interface IBankingService
{
    Task<IReadOnlyList<BankTransaction>> GetAsync(DateOnly? from, DateOnly? to, bool? isMatched, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<BankTransaction>> ImportAsync(Stream csvStream, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<BankTransaction>> SyncExternalAsync(CancellationToken cancellationToken = default);
    Task<BankAutoMatchResult> AutoMatchAsync(CancellationToken cancellationToken = default);
    Task LinkTransactionToInvoiceAsync(Guid transactionId, Guid invoiceId, CancellationToken cancellationToken = default);
}

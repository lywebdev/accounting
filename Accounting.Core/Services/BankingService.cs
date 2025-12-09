using Accounting.Core.Entities;
using Accounting.Core.Interfaces.Integrations;
using Accounting.Core.Interfaces.Repositories;
using Accounting.Core.Interfaces.Services;
using Accounting.Core.Models.Banking;

namespace Accounting.Core.Services;

public class BankingService(
    IBankTransactionRepository transactionRepository,
    IInvoiceRepository invoiceRepository,
    IBankFeedClient bankFeedClient,
    IBankStatementImporter bankStatementImporter,
    IBankAutoMatchService autoMatchService) : IBankingService
{

    public Task<IReadOnlyList<BankTransaction>> GetAsync(DateOnly? from, DateOnly? to, bool? isMatched, string? searchTerm, decimal? amountMin, decimal? amountMax, CancellationToken cancellationToken = default)
        => transactionRepository.GetAsync(from, to, isMatched, searchTerm, amountMin, amountMax, cancellationToken);

    public async Task<IReadOnlyList<BankTransaction>> ImportAsync(Stream csvStream, CancellationToken cancellationToken = default)
    {
        var transactions = await bankStatementImporter.ParseAsync(csvStream, cancellationToken);
        foreach (var transaction in transactions)
        {
            await transactionRepository.AddAsync(transaction, cancellationToken);
        }

        return transactions;
    }

    public async Task<IReadOnlyList<BankTransaction>> SyncExternalAsync(CancellationToken cancellationToken = default)
    {
        var feedTransactions = await bankFeedClient.FetchLatestAsync();
        foreach (var transaction in feedTransactions)
        {
            await transactionRepository.AddAsync(transaction, cancellationToken);
        }

        return feedTransactions;
    }

    public Task<BankAutoMatchResult> AutoMatchAsync(CancellationToken cancellationToken = default)
        => autoMatchService.AutoMatchAsync(cancellationToken);

    public async Task LinkTransactionToInvoiceAsync(Guid transactionId, Guid invoiceId, CancellationToken cancellationToken = default)
    {
        var transaction = await transactionRepository.GetByIdAsync(transactionId, cancellationToken) ?? throw new InvalidOperationException("Transaction not found");
        var invoice = await invoiceRepository.GetByIdAsync(invoiceId, cancellationToken) ?? throw new InvalidOperationException("Invoice not found");
        transaction.LinkToInvoice(invoice.Id);
        await transactionRepository.UpdateAsync(transaction, cancellationToken);
    }

}

using Accounting.Core.Entities;
using Accounting.Core.Enums;
using Accounting.Core.Interfaces.Repositories;
using Accounting.Core.Interfaces.Services;
using Accounting.Core.Models.Banking;
using Accounting.Core.Options;
using Accounting.Core.ValueObjects;
using Microsoft.Extensions.Options;

namespace Accounting.Core.Services;

public class BankAutoMatchService(
    IBankTransactionRepository transactionRepository,
    IInvoiceRepository invoiceRepository,
    IAccountRepository accountRepository,
    IJournalCommandService journalService,
    IOptions<BankingSettings> bankingOptions) : IBankAutoMatchService
{
    private readonly BankingSettings settings = bankingOptions.Value;

    public async Task<BankAutoMatchResult> AutoMatchAsync(CancellationToken cancellationToken = default)
    {
        var unmatchedTransactions = await transactionRepository.GetAsync(null, null, false, null, null, null, cancellationToken);
        if (unmatchedTransactions.Count == 0)
        {
            return BankAutoMatchResult.Empty;
        }

        var invoices = await invoiceRepository.GetAsync(null, null, null, searchTerm: null, cancellationToken);
        var postedInvoices = invoices.Where(i => i.IsPosted).ToList();
        if (postedInvoices.Count == 0)
        {
            return BankAutoMatchResult.Empty;
        }

        var alreadyMatchedInvoiceIds = (await transactionRepository.GetAsync(null, null, true, null, null, null, cancellationToken))
            .Where(t => t.MatchedInvoiceId.HasValue)
            .Select(t => t.MatchedInvoiceId!.Value)
            .ToHashSet();

        var matchedTransactions = new List<BankTransaction>();
        var journalEntries = 0;

        foreach (var transaction in unmatchedTransactions)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var invoice = TryFindMatch(transaction, postedInvoices, alreadyMatchedInvoiceIds);
            if (invoice is null)
            {
                continue;
            }

            transaction.LinkToInvoice(invoice.Id);
            var journalId = await TryCreateSettlementEntryAsync(transaction, invoice, cancellationToken);
            if (journalId.HasValue)
            {
                transaction.LinkToJournalEntry(journalId.Value);
                journalEntries++;
            }

            await transactionRepository.UpdateAsync(transaction, cancellationToken);
            matchedTransactions.Add(transaction);
            alreadyMatchedInvoiceIds.Add(invoice.Id);
        }

        return new BankAutoMatchResult(matchedTransactions.Count, journalEntries, matchedTransactions);
    }

    private Invoice? TryFindMatch(BankTransaction transaction, IEnumerable<Invoice> invoices, HashSet<Guid> usedInvoiceIds)
    {
        var absoluteAmount = Math.Abs(transaction.Amount.Amount);
        var isIncoming = transaction.Amount.Amount >= 0;

        bool IsAmountClose(Money total) => Math.Abs(total.Amount - absoluteAmount) < 0.01m;

        var candidates = invoices.Where(i =>
                !usedInvoiceIds.Contains(i.Id) &&
                IsAmountClose(i.TotalGross(transaction.Amount.Currency)) &&
                ((i.Type == InvoiceType.Sales && isIncoming) || (i.Type == InvoiceType.Purchase && !isIncoming)))
            .ToList();

        if (candidates.Count == 0)
        {
            return null;
        }

        string reference = transaction.Reference ?? string.Empty;
        var referenceMatch = candidates.FirstOrDefault(i => reference.Contains(i.Number, StringComparison.OrdinalIgnoreCase));
        if (referenceMatch is not null)
        {
            return referenceMatch;
        }

        string counterparty = transaction.Counterparty ?? string.Empty;
        var counterpartyMatch = candidates.FirstOrDefault(i => counterparty.Contains(i.Counterparty, StringComparison.OrdinalIgnoreCase));
        if (counterpartyMatch is not null)
        {
            return counterpartyMatch;
        }

        return candidates.Count == 1 ? candidates[0] : null;
    }

    private async Task<Guid?> TryCreateSettlementEntryAsync(BankTransaction transaction, Invoice invoice, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(settings.BankAccountNumber))
        {
            return null;
        }

        var bankAccount = await accountRepository.GetByNumberAsync(settings.BankAccountNumber, cancellationToken);
        if (bankAccount is null)
        {
            return null;
        }

        var offsetNumber = invoice.Type == InvoiceType.Sales ? settings.AccountsReceivableNumber : settings.AccountsPayableNumber;
        if (string.IsNullOrWhiteSpace(offsetNumber))
        {
            return null;
        }

        var offsetAccount = await accountRepository.GetByNumberAsync(offsetNumber, cancellationToken);
        if (offsetAccount is null)
        {
            return null;
        }

        var amount = Math.Abs(transaction.Amount.Amount);
        var lines = invoice.Type == InvoiceType.Sales
            ? new[]
            {
                new JournalEntryLine(bankAccount.Id, amount, 0m, $"Bank receipt {invoice.Number}"),
                new JournalEntryLine(offsetAccount.Id, 0m, amount, $"Settlement of {invoice.Number}")
            }
            : new[]
            {
                new JournalEntryLine(offsetAccount.Id, amount, 0m, $"Settlement of {invoice.Number}"),
                new JournalEntryLine(bankAccount.Id, 0m, amount, $"Bank payment {invoice.Number}")
            };

        var entry = await journalService.CreateAsync($"BANK-{transaction.BookingDate:yyyyMMdd}", transaction.BookingDate, $"Auto-match {invoice.Number}", lines, cancellationToken);
        return entry.Id;
    }
}

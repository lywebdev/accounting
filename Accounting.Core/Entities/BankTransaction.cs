using System.Diagnostics.CodeAnalysis;
using Accounting.Core.Constants;
using Accounting.Core.ValueObjects;

namespace Accounting.Core.Entities;

public class BankTransaction
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public DateOnly BookingDate { get; private set; }
    public string Counterparty { get; private set; }
    public string Reference { get; private set; }
    public Money Amount { get; private set; }
    public Guid? MatchedInvoiceId { get; private set; }
    public Guid? JournalEntryId { get; private set; }

    public BankTransaction(DateOnly bookingDate, string? counterparty, string? reference, Money amount)
    {
        BookingDate = bookingDate;
        Counterparty = counterparty ?? throw new ArgumentNullException(nameof(counterparty));
        Reference = reference ?? string.Empty;
        Amount = amount;
    }

    public void LinkToInvoice(Guid invoiceId) => MatchedInvoiceId = invoiceId;
    public void LinkToJournalEntry(Guid journalEntryId) => JournalEntryId = journalEntryId;

#pragma warning disable IDE0051 // EF Core requires parameterless constructor
    private BankTransaction()
    {
        Counterparty = string.Empty;
        Reference = string.Empty;
        Amount = Money.Zero(CurrencyCodes.Euro);
    }
#pragma warning restore IDE0051
}


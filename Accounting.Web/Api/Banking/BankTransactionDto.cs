using Accounting.Core.Entities;

namespace Accounting.Web.Api.Banking;

public sealed record BankTransactionDto(
    Guid Id,
    DateOnly BookingDate,
    string Counterparty,
    string Reference,
    decimal Amount,
    Guid? MatchedInvoiceId,
    Guid? JournalEntryId);

public sealed record AutoMatchResultDto(int Matched, int JournalEntriesCreated);

internal static class BankTransactionMappings
{
    public static BankTransactionDto ToDto(this BankTransaction transaction) =>
        new(transaction.Id, transaction.BookingDate, transaction.Counterparty, transaction.Reference, transaction.Amount.Amount, transaction.MatchedInvoiceId, transaction.JournalEntryId);
}

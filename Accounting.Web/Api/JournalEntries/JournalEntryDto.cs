using Accounting.Core.Entities;

namespace Accounting.Web.Api.JournalEntries;

public sealed record JournalEntryDto(
    Guid Id,
    string Reference,
    DateOnly EntryDate,
    string? Memo,
    decimal TotalDebit,
    decimal TotalCredit,
    IReadOnlyCollection<JournalEntryLineDto> Lines);

public sealed record JournalEntryLineDto(
    Guid AccountId,
    decimal Debit,
    decimal Credit,
    string Description);

internal static class JournalEntryMappings
{
    public static JournalEntryDto ToDto(this JournalEntry entry) =>
        new(entry.Id, entry.Reference, entry.EntryDate, entry.Memo, entry.TotalDebit, entry.TotalCredit,
            entry.Lines.Select(l => new JournalEntryLineDto(l.AccountId, l.Debit, l.Credit, l.Description)).ToList());
}

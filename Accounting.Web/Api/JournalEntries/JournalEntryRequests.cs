using System.ComponentModel.DataAnnotations;

namespace Accounting.Web.Api.JournalEntries;

public sealed record CreateJournalEntryRequest(
    [Required] string Reference,
    DateOnly EntryDate,
    string? Memo,
    [MinLength(2)] IReadOnlyCollection<JournalEntryLineRequest> Lines);

public sealed record JournalEntryLineRequest(
    Guid AccountId,
    decimal Debit,
    decimal Credit,
    string Description);

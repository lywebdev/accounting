using Accounting.Core.Entities;

namespace Accounting.Core.Models.Banking;

public sealed record BankAutoMatchResult(int MatchedTransactions, int JournalEntriesCreated, IReadOnlyList<BankTransaction> UpdatedTransactions)
{
    public static BankAutoMatchResult Empty { get; } = new(0, 0, Array.Empty<BankTransaction>());
}

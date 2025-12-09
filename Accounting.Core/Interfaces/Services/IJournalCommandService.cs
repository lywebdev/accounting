using Accounting.Core.Entities;

namespace Accounting.Core.Interfaces.Services;

public interface IJournalCommandService
{
    Task<JournalEntry> CreateAsync(string reference, DateOnly entryDate, string? memo, IEnumerable<JournalEntryLine> lines, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

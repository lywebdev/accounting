using Accounting.Core.Entities;

namespace Accounting.Core.Interfaces.Services;

public interface IJournalService
{
    Task<JournalEntry> CreateAsync(string reference, DateOnly entryDate, string? memo, IEnumerable<JournalEntryLine> lines, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<JournalEntry>> GetAsync(DateOnly? from, DateOnly? to, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<JournalEntry> PostAsync(Guid id, CancellationToken cancellationToken = default);
}

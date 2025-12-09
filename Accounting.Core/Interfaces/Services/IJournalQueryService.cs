using Accounting.Core.Entities;

namespace Accounting.Core.Interfaces.Services;

public interface IJournalQueryService
{
    Task<IReadOnlyList<JournalEntry>> GetAsync(DateOnly? from, DateOnly? to, CancellationToken cancellationToken = default);
}

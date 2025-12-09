using Accounting.Core.Entities;
using Accounting.Core.Interfaces.Repositories;
using Accounting.Core.Interfaces.Services;
using FluentValidation;

namespace Accounting.Core.Services;

public class JournalService(IJournalEntryRepository repository, IValidator<JournalEntry> validator)
    : IJournalQueryService, IJournalCommandService
{
    public Task<IReadOnlyList<JournalEntry>> GetAsync(DateOnly? from, DateOnly? to, CancellationToken cancellationToken = default)
        => repository.GetAsync(from, to, cancellationToken);

    public async Task<JournalEntry> CreateAsync(string reference, DateOnly entryDate, string? memo, IEnumerable<JournalEntryLine> lines, CancellationToken cancellationToken = default)
    {
        var entry = new JournalEntry(reference, entryDate, memo);
        foreach (var line in lines)
        {
            entry.AddLine(line);
        }

        await validator.ValidateAndThrowAsync(entry, cancellationToken);
        await repository.AddAsync(entry, cancellationToken);
        return entry;
    }

    public Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        => repository.DeleteAsync(id, cancellationToken);

}

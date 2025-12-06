using Accounting.Core.Entities;
using Accounting.Core.Enums;

namespace Accounting.Core.Interfaces.Services;

public interface IChartOfAccountsService
{
    Task<IReadOnlyList<Account>> GetAsync(AccountCategory? category, CancellationToken cancellationToken = default);
    Task<Account> CreateAsync(string number, string name, AccountCategory category, string? description, CancellationToken cancellationToken = default);
    Task<Account> UpdateAsync(Guid id, string name, AccountCategory category, string? description, CancellationToken cancellationToken = default);
    Task DeactivateAsync(Guid id, CancellationToken cancellationToken = default);
    Task ActivateAsync(Guid id, CancellationToken cancellationToken = default);
}

using Accounting.Core.Entities;
using Accounting.Core.Enums;

namespace Accounting.Core.Interfaces.Services;

public interface IChartOfAccountsQueryService
{
    Task<IReadOnlyList<Account>> GetAsync(AccountCategory? category, CancellationToken cancellationToken = default);
    Task<Account?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}

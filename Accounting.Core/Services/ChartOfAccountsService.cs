using Accounting.Core.Entities;
using Accounting.Core.Enums;
using Accounting.Core.Interfaces.Repositories;
using Accounting.Core.Interfaces.Services;
using FluentValidation;

namespace Accounting.Core.Services;

public class ChartOfAccountsService(IAccountRepository repository, IValidator<Account> validator)
    : IChartOfAccountsQueryService, IChartOfAccountsCommandService
{
    public Task<IReadOnlyList<Account>> GetAsync(AccountCategory? category, CancellationToken cancellationToken = default)
        => repository.GetAsync(category, cancellationToken);

    public Task<Account?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => repository.GetByIdAsync(id, cancellationToken);

    public async Task<Account> CreateAsync(string number, string name, AccountCategory category, string? description, CancellationToken cancellationToken = default)
    {
        var account = new Account(number, name, category, description);
        await validator.ValidateAndThrowAsync(account, cancellationToken);
        await repository.AddAsync(account, cancellationToken);
        return account;
    }

    public async Task<Account> UpdateAsync(Guid id, string name, AccountCategory category, string? description, CancellationToken cancellationToken = default)
    {
        var account = await repository.GetByIdAsync(id, cancellationToken) ?? throw new InvalidOperationException("Account not found");
        account.Update(name, category, description);
        await validator.ValidateAndThrowAsync(account, cancellationToken);
        await repository.UpdateAsync(account, cancellationToken);
        return account;
    }

    public async Task DeactivateAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var account = await repository.GetByIdAsync(id, cancellationToken) ?? throw new InvalidOperationException("Account not found");
        account.Deactivate();
        await repository.UpdateAsync(account, cancellationToken);
    }

    public async Task ActivateAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var account = await repository.GetByIdAsync(id, cancellationToken) ?? throw new InvalidOperationException("Account not found");
        account.Activate();
        await repository.UpdateAsync(account, cancellationToken);
    }
}

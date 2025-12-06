using Accounting.Core.Entities;

namespace Accounting.Core.Interfaces.Repositories;

public interface IBankTransactionRepository
{
    Task<BankTransaction?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<BankTransaction>> GetAsync(DateOnly? from, DateOnly? to, bool? isMatched, CancellationToken cancellationToken = default);
    Task AddAsync(BankTransaction transaction, CancellationToken cancellationToken = default);
    Task UpdateAsync(BankTransaction transaction, CancellationToken cancellationToken = default);
}

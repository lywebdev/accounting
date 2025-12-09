using Accounting.Core.Models.Banking;

namespace Accounting.Core.Interfaces.Services;

public interface IBankAutoMatchService
{
    Task<BankAutoMatchResult> AutoMatchAsync(CancellationToken cancellationToken = default);
}

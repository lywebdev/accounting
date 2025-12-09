using Accounting.Core.Entities;

namespace Accounting.Core.Interfaces.Integrations;

public interface IBankFeedClient
{
    Task<IReadOnlyList<BankTransaction>> FetchLatestAsync();
}

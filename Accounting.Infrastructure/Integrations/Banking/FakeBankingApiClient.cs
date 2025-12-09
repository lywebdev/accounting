using Accounting.Core.Constants;
using Accounting.Core.Entities;
using Accounting.Core.Interfaces.Integrations;
using Accounting.Core.ValueObjects;

namespace Accounting.Infrastructure.Integrations.Banking;

public class FakeBankingApiClient : IBankFeedClient
{
    public Task<IReadOnlyList<BankTransaction>> FetchLatestAsync()
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var random = new Random();
        var transactions = Enumerable.Range(1, 5).Select(index =>
        {
            var amount = Math.Round((decimal)random.NextDouble() * 2000 - 1000, 2);
            var money = new Money(amount, CurrencyCodes.Euro);
            return new BankTransaction(today.AddDays(-index), $"Demo Vendor {index}", $"EXT{today:yyyyMMdd}{index:D3}", money);
        }).ToList();

        return Task.FromResult<IReadOnlyList<BankTransaction>>(transactions);
    }
}

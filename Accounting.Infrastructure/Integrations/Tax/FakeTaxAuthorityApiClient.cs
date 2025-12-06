using Accounting.Core.Interfaces.Integrations;

namespace Accounting.Infrastructure.Integrations.Tax;

public class FakeTaxAuthorityApiClient : ITaxAuthorityClient
{
    public Task<bool> ValidateVatAsync(string vatNumber, CancellationToken cancellationToken = default)
    {
        var isValid = !string.IsNullOrWhiteSpace(vatNumber) && vatNumber.Length >= 10;
        return Task.FromResult(isValid);
    }

    public Task<string> SubmitDeclarationAsync(int year, int period, CancellationToken cancellationToken = default)
    {
        var status = Random.Shared.NextDouble() > 0.2 ? "Accepted" : "Rejected";
        return Task.FromResult($"VAT-{year}-{period:D2}-{status.ToUpperInvariant()}");
    }
}

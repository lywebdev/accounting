namespace Accounting.Core.Interfaces.Integrations;

public interface ITaxAuthorityClient
{
    Task<bool> ValidateVatAsync(string vatNumber, CancellationToken cancellationToken = default);
    Task<string> SubmitDeclarationAsync(int year, int period, CancellationToken cancellationToken = default);
}

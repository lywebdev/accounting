namespace Accounting.Core.Interfaces.Integrations;

public interface ITaxAuthorityClient
{
    Task<bool> ValidateVatAsync(string vatNumber);
    Task<string> SubmitDeclarationAsync(int year, int period);
}

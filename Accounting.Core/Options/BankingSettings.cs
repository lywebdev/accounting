namespace Accounting.Core.Options;

public sealed class BankingSettings
{
    public string? BankAccountNumber { get; init; }
    public string? AccountsReceivableNumber { get; init; }
    public string? AccountsPayableNumber { get; init; }
}

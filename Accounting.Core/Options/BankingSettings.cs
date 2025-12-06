namespace Accounting.Core.Options;

public sealed class BankingSettings
{
    public string? BankAccountNumber { get; set; }
    public string? AccountsReceivableNumber { get; set; }
    public string? AccountsPayableNumber { get; set; }
}

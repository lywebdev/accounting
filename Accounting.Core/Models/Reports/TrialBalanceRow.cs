namespace Accounting.Core.Models.Reports;

public record TrialBalanceRow(string AccountNumber, string AccountName, decimal Debit, decimal Credit);

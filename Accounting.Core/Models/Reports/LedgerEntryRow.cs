namespace Accounting.Core.Models.Reports;

public record LedgerEntryRow(
    DateOnly Date,
    string Reference,
    string AccountNumber,
    string AccountName,
    string Description,
    decimal Debit,
    decimal Credit,
    decimal Balance);

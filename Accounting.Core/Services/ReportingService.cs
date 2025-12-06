using Accounting.Core.Entities;
using Accounting.Core.Enums;
using Accounting.Core.Interfaces.Repositories;
using Accounting.Core.Interfaces.Services;
using Accounting.Core.Models.Reports;

namespace Accounting.Core.Services;

public class ReportingService(
    IAccountRepository accountRepository,
    IJournalEntryRepository journalEntryRepository) : IReportingService
{
    public async Task<ReportResult<TrialBalanceRow>> GetTrialBalanceAsync(ReportFilter filter, CancellationToken cancellationToken = default)
    {
        var accounts = await accountRepository.GetAsync(filter.Category, cancellationToken);
        var journalEntries = await journalEntryRepository.GetAsync(filter.From, filter.To, cancellationToken);
        var lineGroups = journalEntries.SelectMany(j => j.Lines)
            .GroupBy(l => l.AccountId)
            .ToDictionary(g => g.Key, g => new { Debit = g.Sum(l => l.Debit), Credit = g.Sum(l => l.Credit) });

        var rows = accounts.Select(account =>
        {
            lineGroups.TryGetValue(account.Id, out var totals);
            var debit = totals?.Debit ?? 0m;
            var credit = totals?.Credit ?? 0m;
            return new TrialBalanceRow(account.Number, account.Name, debit, credit);
        }).ToList();

        return new ReportResult<TrialBalanceRow>(rows, DateTimeOffset.UtcNow, filter.Currency);
    }

    public async Task<ReportResult<FinancialStatementRow>> GetProfitAndLossAsync(ReportFilter filter, CancellationToken cancellationToken = default)
    {
        var accounts = await accountRepository.GetAsync(null, cancellationToken);
        var journalEntries = await journalEntryRepository.GetAsync(filter.From, filter.To, cancellationToken);
        var revenueAccounts = accounts.Where(a => a.Category == AccountCategory.Revenue).ToList();
        var expenseAccounts = accounts.Where(a => a.Category == AccountCategory.Expense).ToList();
        var lineGroups = journalEntries.SelectMany(j => j.Lines).GroupBy(l => l.AccountId).ToDictionary(g => g.Key, g => new { Debit = g.Sum(l => l.Debit), Credit = g.Sum(l => l.Credit) });

        decimal SumFor(IEnumerable<Account> accs)
            => accs.Sum(a => lineGroups.TryGetValue(a.Id, out var totals)
                ? totals.Credit - totals.Debit
                : 0m);

        var revenue = SumFor(revenueAccounts);
        var expenses = SumFor(expenseAccounts);
        var rows = new List<FinancialStatementRow>
        {
            new("Revenue", revenue),
            new("Expenses", expenses),
            new("Net Income", revenue - expenses)
        };

        return new ReportResult<FinancialStatementRow>(rows, DateTimeOffset.UtcNow, filter.Currency);
    }

    public async Task<ReportResult<FinancialStatementRow>> GetBalanceSheetAsync(ReportFilter filter, CancellationToken cancellationToken = default)
    {
        var accounts = await accountRepository.GetAsync(null, cancellationToken);
        var journalEntries = await journalEntryRepository.GetAsync(filter.From, filter.To, cancellationToken);
        var lineGroups = journalEntries.SelectMany(j => j.Lines).GroupBy(l => l.AccountId).ToDictionary(g => g.Key, g => new { Debit = g.Sum(l => l.Debit), Credit = g.Sum(l => l.Credit) });

        decimal SumCategory(AccountCategory category) => accounts
            .Where(a => a.Category == category)
            .Sum(a => lineGroups.TryGetValue(a.Id, out var totals)
                ? totals.Debit - totals.Credit
                : 0m);

        var rows = new List<FinancialStatementRow>
        {
            new("Assets", SumCategory(AccountCategory.Assets)),
            new("Liabilities", SumCategory(AccountCategory.Liabilities)),
            new("Equity", SumCategory(AccountCategory.Equity))
        };

        return new ReportResult<FinancialStatementRow>(rows, DateTimeOffset.UtcNow, filter.Currency);
    }

    public async Task<ReportResult<LedgerEntryRow>> GetGeneralLedgerAsync(ReportFilter filter, CancellationToken cancellationToken = default)
    {
        var accounts = await accountRepository.GetAsync(null, cancellationToken);
        var accountLookup = accounts.ToDictionary(a => a.Id, a => a);
        var journalEntries = await journalEntryRepository.GetAsync(filter.From, filter.To, cancellationToken);
        var rows = new List<LedgerEntryRow>();
        var balances = new Dictionary<Guid, decimal>();

        foreach (var entry in journalEntries.OrderBy(j => j.EntryDate))
        {
            foreach (var line in entry.Lines)
            {
                if (filter.AccountId.HasValue && line.AccountId != filter.AccountId.Value)
                {
                    continue;
                }

                if (!accountLookup.TryGetValue(line.AccountId, out var account))
                {
                    continue;
                }

                var currentBalance = balances.TryGetValue(line.AccountId, out var balance)
                    ? balance
                    : 0m;
                currentBalance += line.Debit - line.Credit;
                balances[line.AccountId] = currentBalance;

                rows.Add(new LedgerEntryRow(
                    entry.EntryDate,
                    entry.Reference,
                    account.Number,
                    account.Name,
                    line.Description,
                    line.Debit,
                    line.Credit,
                    currentBalance));
            }
        }

        return new ReportResult<LedgerEntryRow>(rows, DateTimeOffset.UtcNow, filter.Currency);
    }
}

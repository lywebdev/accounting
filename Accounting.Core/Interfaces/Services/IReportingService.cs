using Accounting.Core.Models.Reports;

namespace Accounting.Core.Interfaces.Services;

public interface IReportingService
{
    Task<ReportResult<TrialBalanceRow>> GetTrialBalanceAsync(ReportFilter filter, CancellationToken cancellationToken = default);
    Task<ReportResult<FinancialStatementRow>> GetProfitAndLossAsync(ReportFilter filter, CancellationToken cancellationToken = default);
    Task<ReportResult<FinancialStatementRow>> GetBalanceSheetAsync(ReportFilter filter, CancellationToken cancellationToken = default);
    Task<ReportResult<LedgerEntryRow>> GetGeneralLedgerAsync(ReportFilter filter, CancellationToken cancellationToken = default);
}

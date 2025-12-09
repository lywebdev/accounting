using Accounting.Core.Constants;
using Accounting.Core.Enums;
using Accounting.Core.Interfaces.Services;
using Accounting.Core.Models.Reports;

namespace Accounting.Web.Api.Reporting;

public static class ReportingEndpoints
{
    public static IEndpointRouteBuilder MapReportingEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/reporting").WithTags("Reporting");

        group.MapGet("/profit-and-loss", async (DateOnly from, DateOnly to, string? currency, IReportingService service, CancellationToken ct) =>
        {
            var filter = new ReportFilter(from, to, Currency: currency ?? CurrencyCodes.Euro);
            var result = await service.GetProfitAndLossAsync(filter, ct);
            return Results.Ok(result);
        });

        group.MapGet("/balance-sheet", async (DateOnly from, DateOnly to, string? currency, IReportingService service, CancellationToken ct) =>
        {
            var filter = new ReportFilter(from, to, Currency: currency ?? CurrencyCodes.Euro);
            var result = await service.GetBalanceSheetAsync(filter, ct);
            return Results.Ok(result);
        });

        group.MapGet("/trial-balance", async (DateOnly from, DateOnly to, AccountCategory? category, string? currency, IReportingService service, CancellationToken ct) =>
        {
            var filter = new ReportFilter(from, to, Category: category, Currency: currency ?? CurrencyCodes.Euro);
            var result = await service.GetTrialBalanceAsync(filter, ct);
            return Results.Ok(result);
        });

        group.MapGet("/general-ledger", async (DateOnly from, DateOnly to, Guid? accountId, string? currency, IReportingService service, CancellationToken ct) =>
        {
            var filter = new ReportFilter(from, to, AccountId: accountId, Currency: currency ?? CurrencyCodes.Euro);
            var result = await service.GetGeneralLedgerAsync(filter, ct);
            return Results.Ok(result);
        });

        return app;
    }
}

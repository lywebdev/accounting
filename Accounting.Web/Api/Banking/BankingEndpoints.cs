using Accounting.Core.Interfaces.Services;

namespace Accounting.Web.Api.Banking;

public static class BankingEndpoints
{
    public static IEndpointRouteBuilder MapBankingEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/banking").WithTags("Banking");

        group.MapGet("/transactions", async (DateOnly? from, DateOnly? to, bool? matched, string? search, decimal? amountMin, decimal? amountMax, IBankingService service, CancellationToken ct) =>
        {
            var transactions = await service.GetAsync(from, to, matched, search, amountMin, amountMax, ct);
            return Results.Ok(transactions.Select(t => t.ToDto()));
        });

        group.MapPost("/transactions/import", async (IBankingService service, HttpRequest request, CancellationToken ct) =>
        {
            if (!request.HasFormContentType || request.Form.Files.Count == 0)
            {
                return Results.BadRequest("CSV file is required.");
            }

            var file = request.Form.Files[0];
            await using var stream = file.OpenReadStream();
            var transactions = await service.ImportAsync(stream, ct);
            return Results.Ok(transactions.Select(t => t.ToDto()));
        });

        group.MapPost("/transactions/{transactionId:guid}/link/{invoiceId:guid}", async (Guid transactionId, Guid invoiceId, IBankingService service, CancellationToken ct) =>
        {
            await service.LinkTransactionToInvoiceAsync(transactionId, invoiceId, ct);
            return Results.NoContent();
        });

        group.MapPost("/transactions/sync", async (IBankingService service, CancellationToken ct) =>
        {
            var transactions = await service.SyncExternalAsync(ct);
            return Results.Ok(transactions.Select(t => t.ToDto()));
        });

        group.MapPost("/transactions/auto-match", async (IBankingService service, CancellationToken ct) =>
        {
            var result = await service.AutoMatchAsync(ct);
            return Results.Ok(new AutoMatchResultDto(result.MatchedTransactions, result.JournalEntriesCreated));
        });

        return app;
    }
}

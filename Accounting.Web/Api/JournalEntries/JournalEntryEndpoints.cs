using Accounting.Core.Entities;
using Accounting.Core.Interfaces.Services;

namespace Accounting.Web.Api.JournalEntries;

public static class JournalEntryEndpoints
{
    public static IEndpointRouteBuilder MapJournalEntryEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/journal-entries").WithTags("Journal Entries");

        group.MapGet("/", async (DateOnly? from, DateOnly? to, IJournalQueryService service, CancellationToken ct) =>
        {
            var entries = await service.GetAsync(from, to, ct);
            return Results.Ok(entries.Select(e => e.ToDto()));
        });

        group.MapPost("/", async (CreateJournalEntryRequest request, IJournalCommandService service, CancellationToken ct) =>
        {
            var lines = request.Lines.Select(l => new JournalEntryLine(l.AccountId, l.Debit, l.Credit, l.Description));
            var entry = await service.CreateAsync(request.Reference, request.EntryDate, request.Memo, lines, ct);
            return Results.Created($"/api/journal-entries/{entry.Id}", entry.ToDto());
        });

        group.MapDelete("/{id:guid}", async (Guid id, IJournalCommandService service, CancellationToken ct) =>
        {
            await service.DeleteAsync(id, ct);
            return Results.NoContent();
        });

        return app;
    }
}

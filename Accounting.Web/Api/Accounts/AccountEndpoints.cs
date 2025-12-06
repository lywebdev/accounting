using Accounting.Core.Enums;
using Accounting.Core.Interfaces.Services;

namespace Accounting.Web.Api.Accounts;

public static class AccountEndpoints
{
    public static IEndpointRouteBuilder MapAccountEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/accounts").WithTags("Accounts");

        group.MapGet("/", async (AccountCategory? category, IChartOfAccountsService service, CancellationToken ct) =>
        {
            var accounts = await service.GetAsync(category, ct);
            return Results.Ok(accounts.Select(a => a.ToDto()));
        });

        group.MapPost("/", async (CreateAccountRequest request, IChartOfAccountsService service, CancellationToken ct) =>
        {
            var account = await service.CreateAsync(request.Number, request.Name, request.Category, request.Description, ct);
            return Results.Created($"/api/accounts/{account.Id}", account.ToDto());
        });

        group.MapPut("/{id:guid}", async (Guid id, UpdateAccountRequest request, IChartOfAccountsService service, CancellationToken ct) =>
        {
            var account = await service.UpdateAsync(id, request.Name, request.Category, request.Description, ct);
            return Results.Ok(account.ToDto());
        });

        group.MapDelete("/{id:guid}", async (Guid id, IChartOfAccountsService service, CancellationToken ct) =>
        {
            await service.DeactivateAsync(id, ct);
            return Results.NoContent();
        });

        group.MapPost("/{id:guid}/activate", async (Guid id, IChartOfAccountsService service, CancellationToken ct) =>
        {
            await service.ActivateAsync(id, ct);
            return Results.NoContent();
        });

        return app;
    }
}

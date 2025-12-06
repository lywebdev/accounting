using Accounting.Core.Enums;
using Accounting.Core.Interfaces.Services;

namespace Accounting.Web.Api.Tax;

public static class TaxEndpoints
{
    public static IEndpointRouteBuilder MapTaxEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/tax").WithTags("VAT");

        group.MapGet("/declaration", async (int year, int period, ITaxService service, CancellationToken ct) =>
        {
            if (!IsValidPeriod(period))
            {
                return Results.BadRequest("Period must be between 1 and 4.");
            }

            var declaration = await service.CalculateAsync(year, period, ct);
            return Results.Ok(declaration.ToDto());
        });

        group.MapPost("/declaration/submit", async (SubmitTaxDeclarationRequest request, ITaxService service, CancellationToken ct) =>
        {
            if (!IsValidPeriod(request.Period))
            {
                return Results.BadRequest("Period must be between 1 and 4.");
            }

            var declaration = await service.SubmitAsync(request.Year, request.Period, ct);
            return Results.Ok(declaration.ToDto());
        });

        group.MapGet("/declarations/{year:int}", async (int year, ITaxService service, CancellationToken ct) =>
        {
            var declarations = await service.GetYearAsync(year, ct);
            return Results.Ok(declarations.Select(d => d.ToDto()));
        });

        group.MapPost("/validate", async (VatValidationRequest request, ITaxService service, CancellationToken ct) =>
        {
            if (string.IsNullOrWhiteSpace(request.VatNumber))
            {
                return Results.BadRequest("VAT number is required.");
            }

            var result = await service.ValidateVatNumberAsync(request.VatNumber, ct);
            return Results.Ok(new VatValidationResponse(result));
        });

        return app;
    }

    private static bool IsValidPeriod(int period) => period is >= 1 and <= 4;
}

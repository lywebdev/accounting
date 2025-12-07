using Accounting.Core.Entities;
using Accounting.Core.Enums;
using Accounting.Core.Interfaces.Services;
using Accounting.Core.ValueObjects;

namespace Accounting.Web.Api.Invoices;

public static class InvoiceEndpoints
{
    public static IEndpointRouteBuilder MapInvoiceEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/invoices").WithTags("Invoices");

        group.MapGet("/", async (InvoiceType? type, DateOnly? from, DateOnly? to, IInvoiceService service, CancellationToken ct) =>
        {
            var invoices = await service.GetAsync(type, from, to, ct);
            return Results.Ok(invoices.Select(i => i.ToDto()));
        });

        group.MapPost("/", async (CreateInvoiceRequest request, IInvoiceService service, CancellationToken ct) =>
        {
            var lines = request.Lines.Select(l => new InvoiceLine(l.Description, l.Quantity, new Money(l.UnitPrice, "EUR"), l.VatRate, l.RevenueAccountId));
            var invoice = await service.CreateAsync(request.Type, request.Number, request.Counterparty, request.IssueDate, request.DueDate, lines, ct);
            return Results.Created($"/api/invoices/{invoice.Id}", invoice.ToDto());
        });

        group.MapPost("/{id:guid}/post", async (Guid id, IInvoiceService service, CancellationToken ct) =>
        {
            var invoice = await service.PostAsync(id, ct);
            return Results.Ok(invoice.ToDto());
        });

        group.MapDelete("/{id:guid}", async (Guid id, IInvoiceService service, CancellationToken ct) =>
        {
            await service.DeleteAsync(id, ct);
            return Results.NoContent();
        });

        group.MapGet("/{id:guid}/pdf", async (Guid id, IInvoiceService service, IInvoiceDocumentService documentService, CancellationToken ct) =>
        {
            var invoice = await service.GetByIdAsync(id, ct);
            var pdf = await documentService.GeneratePdfAsync(invoice, ct);
            var fileName = $"invoice-{invoice.Number}.pdf";
            return Results.File(pdf, "application/pdf", fileName);
        });

        return app;
    }
}

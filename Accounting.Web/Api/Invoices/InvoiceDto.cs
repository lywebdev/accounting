using Accounting.Core.Entities;
using Accounting.Core.Enums;

namespace Accounting.Web.Api.Invoices;

public sealed record InvoiceDto(
    Guid Id,
    InvoiceType Type,
    string Number,
    string Counterparty,
    DateOnly IssueDate,
    DateOnly DueDate,
    bool IsPosted,
    decimal TotalNet,
    decimal TotalVat,
    decimal TotalGross,
    IReadOnlyCollection<InvoiceLineDto> Lines);

public sealed record InvoiceLineDto(
    Guid Id,
    string Description,
    decimal Quantity,
    decimal UnitPrice,
    decimal VatRate,
    decimal LineNet,
    decimal LineVat,
    decimal LineTotal);

internal static class InvoiceMappings
{
    public static InvoiceDto ToDto(this Invoice invoice)
    {
        const string currency = "EUR";
        return new InvoiceDto(
            invoice.Id,
            invoice.Type,
            invoice.Number,
            invoice.Counterparty,
            invoice.IssueDate,
            invoice.DueDate,
            invoice.IsPosted,
            invoice.TotalNet(currency).Amount,
            invoice.TotalVat(currency).Amount,
            invoice.TotalGross(currency).Amount,
            invoice.Lines.Select(l => new InvoiceLineDto(
                l.Id,
                l.Description,
                l.Quantity,
                l.UnitPrice.Amount,
                l.VatRate,
                l.NetAmount.Amount,
                l.VatAmount.Amount,
                l.Total.Amount)).ToList());
    }
}

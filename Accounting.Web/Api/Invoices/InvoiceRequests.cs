using System.ComponentModel.DataAnnotations;
using Accounting.Core.Enums;

namespace Accounting.Web.Api.Invoices;

public sealed record CreateInvoiceRequest(
    [Required] InvoiceType Type,
    [Required, StringLength(32)] string Number,
    [Required, StringLength(128)] string Counterparty,
    DateOnly IssueDate,
    DateOnly DueDate,
    [MinLength(1)] IReadOnlyCollection<InvoiceLineRequest> Lines);

public sealed record InvoiceLineRequest(
    Guid RevenueAccountId,
    decimal Quantity,
    decimal UnitPrice,
    decimal VatRate,
    [StringLength(256)] string Description);

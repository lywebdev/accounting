using Accounting.Core.Constants;
using Accounting.Core.Entities;
using Accounting.Core.Interfaces.Services;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Accounting.Infrastructure.Documents;

public class InvoiceDocumentService : IInvoiceDocumentService
{
    static InvoiceDocumentService()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public Task<byte[]> GeneratePdfAsync(Invoice invoice)
    {
        var stream = new MemoryStream();
        var currency = CurrencyCodes.Euro;
        Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(40);
                page.Header().Row(row =>
                {
                    row.Spacing(10);
                    row.RelativeItem().Column(column =>
                    {
                        column.Item().Text("AccountingApp").FontSize(18).SemiBold();
                        column.Item().Text($"Invoice #{invoice.Number}");
                        column.Item().Text($"Counterparty: {invoice.Counterparty}");
                    });
                    row.ConstantItem(120).AlignRight().Text($"Issue: {invoice.IssueDate:yyyy-MM-dd}").FontSize(11);
                });

                page.Content().PaddingTop(10).Column(column =>
                {
                    column.Spacing(10);
                    column.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn();
                            columns.RelativeColumn(0.5f);
                            columns.RelativeColumn(0.5f);
                            columns.RelativeColumn(0.5f);
                            columns.RelativeColumn(0.6f);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(CellStyle).Text("Description");
                            header.Cell().Element(CellStyle).Text("Qty");
                            header.Cell().Element(CellStyle).Text("Unit");
                            header.Cell().Element(CellStyle).Text("VAT %");
                            header.Cell().Element(CellStyle).AlignRight().Text("Total");

                            static IContainer CellStyle(IContainer container) =>
                                container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).Background(Colors.Grey.Lighten3).BorderBottom(1);
                        });

                        foreach (var line in invoice.Lines)
                        {
                            var total = line.Quantity * line.UnitPrice.Amount * (1 + (line.VatRate / 100m));
                            table.Cell().PaddingVertical(4).Text(line.Description);
                            table.Cell().PaddingVertical(4).Text(line.Quantity.ToString("0.##"));
                            table.Cell().PaddingVertical(4).Text(line.UnitPrice.Amount.ToString("0.00"));
                            table.Cell().PaddingVertical(4).Text($"{line.VatRate:0.##}%");
                            table.Cell().PaddingVertical(4).AlignRight().Text($"{total:0.00} {currency}");
                        }
                    });

                    column.Item().AlignRight().Text($"Total gross: {invoice.TotalGross(currency).Amount:0.00} {currency}").FontSize(14).SemiBold();
                    column.Item().Text(invoice.IsPosted ? "This invoice has been posted to the ledger." : "Status: Draft");
                });

                page.Footer().AlignRight().Text(text =>
                {
                    text.Span("Generated ").FontSize(10);
                    text.Span(DateTimeOffset.UtcNow.ToString("g")).FontSize(10);
                });
            });
        }).GeneratePdf(stream);

        return Task.FromResult(stream.ToArray());
    }
}

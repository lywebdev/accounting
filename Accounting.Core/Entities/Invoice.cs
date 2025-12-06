using Accounting.Core.Enums;
using Accounting.Core.ValueObjects;

namespace Accounting.Core.Entities;

public class Invoice
{
    private readonly List<InvoiceLine> _lines = new();

    public Guid Id { get; private set; } = Guid.NewGuid();
    public InvoiceType Type { get; private set; }
    public string Number { get; private set; }
    public string Counterparty { get; private set; }
    public DateOnly IssueDate { get; private set; }
    public DateOnly DueDate { get; private set; }
    public bool IsPosted { get; private set; }
    public IReadOnlyCollection<InvoiceLine> Lines => _lines.AsReadOnly();

    public Invoice(InvoiceType type, string number, string counterparty, DateOnly issueDate, DateOnly dueDate)
    {
        Type = type;
        Number = number ?? throw new ArgumentNullException(nameof(number));
        Counterparty = counterparty ?? throw new ArgumentNullException(nameof(counterparty));
        IssueDate = issueDate;
        DueDate = dueDate;
    }

    public void AddLine(InvoiceLine line)
    {
        ArgumentNullException.ThrowIfNull(line);
        line.AssignInvoice(Id);
        _lines.Add(line);
    }

    public Money TotalNet(string currency) => _lines.Aggregate(Money.Zero(currency), (acc, line) => acc.Add(line.NetAmount));
    public Money TotalVat(string currency) => _lines.Aggregate(Money.Zero(currency), (acc, line) => acc.Add(line.VatAmount));
    public Money TotalGross(string currency) => TotalNet(currency).Add(TotalVat(currency));

    public void MarkPosted()
    {
        if (!_lines.Any())
        {
            throw new InvalidOperationException("Invoice must have at least one line.");
        }

        IsPosted = true;
    }

    private Invoice()
    {
        Number = string.Empty;
        Counterparty = string.Empty;
    }
}

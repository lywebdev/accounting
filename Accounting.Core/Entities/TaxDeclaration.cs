using Accounting.Core.Enums;

namespace Accounting.Core.Entities;

public class TaxDeclaration
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public int Year { get; private set; }
    public int Period { get; private set; }
    public decimal VatPayable { get; private set; }
    public decimal VatReceivable { get; private set; }
    public TaxDeclarationStatus Status { get; private set; } = TaxDeclarationStatus.Draft;
    public DateTimeOffset? SubmittedAt { get; private set; }

    public TaxDeclaration(int year, int period)
    {
        Year = year;
        Period = period;
    }

    public void SetFigures(decimal payable, decimal receivable)
    {
        VatPayable = payable;
        VatReceivable = receivable;
    }

    public void MarkSubmitted(TaxDeclarationStatus status)
    {
        if (status is TaxDeclarationStatus.Draft)
        {
            throw new ArgumentException("Submitted declarations must not be draft.", nameof(status));
        }

        Status = status;
        SubmittedAt = DateTimeOffset.UtcNow;
    }

    private TaxDeclaration()
    {
    }
}

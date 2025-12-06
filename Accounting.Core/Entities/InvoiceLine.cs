using Accounting.Core.ValueObjects;

namespace Accounting.Core.Entities;

public class InvoiceLine
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid InvoiceId { get; private set; }
    public string Description { get; private set; }
    public decimal Quantity { get; private set; }
    public Money UnitPrice { get; private set; }
    public decimal VatRate { get; private set; }
    public Guid RevenueAccountId { get; private set; }

    public InvoiceLine(string description, decimal quantity, Money unitPrice, decimal vatRate, Guid revenueAccountId)
    {
        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity));
        }

        Description = description ?? string.Empty;
        Quantity = quantity;
        UnitPrice = unitPrice;
        VatRate = vatRate;
        RevenueAccountId = revenueAccountId;
    }

    public Money NetAmount => new(UnitPrice.Amount * Quantity, UnitPrice.Currency);
    public Money VatAmount => new(decimal.Round(NetAmount.Amount * VatRate / 100m, 2, MidpointRounding.AwayFromZero), UnitPrice.Currency);
    public Money Total => NetAmount.Add(VatAmount);

    internal void AssignInvoice(Guid invoiceId) => InvoiceId = invoiceId;

    private InvoiceLine()
    {
        Description = string.Empty;
        UnitPrice = Money.Zero("EUR");
    }
}

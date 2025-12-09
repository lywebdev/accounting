using System.Globalization;
using Accounting.Core.Constants;

namespace Accounting.Core.ValueObjects;

public record Money
{
    public Money(decimal amount, string currency)
    {
        Amount = decimal.Round(amount, 2, MidpointRounding.AwayFromZero);
        Currency = (currency ?? CurrencyCodes.Euro).ToUpperInvariant();
    }

    public decimal Amount { get; }
    public string Currency { get; }

    public static Money Zero(string currency) => new(0m, currency);

    public Money Add(Money other)
    {
        EnsureSameCurrency(other);
        return new Money(Amount + other.Amount, Currency);
    }

    public Money Subtract(Money other)
    {
        EnsureSameCurrency(other);
        return new Money(Amount - other.Amount, Currency);
    }

    public override string ToString() => string.Create(CultureInfo.InvariantCulture, $"{Amount:0.00} {Currency}");

    private void EnsureSameCurrency(Money other)
    {
        if (!Currency.Equals(other.Currency, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Currency mismatch detected.");
        }
    }
}

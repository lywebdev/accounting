namespace Accounting.Core.ValueObjects;

public readonly record struct VatRate(decimal Percentage)
{
    public decimal Percentage { get; init; } = decimal.Round(Percentage, 2, MidpointRounding.AwayFromZero);

    public decimal Calculate(decimal netAmount) => decimal.Round(netAmount * (Percentage / 100m), 2, MidpointRounding.AwayFromZero);
}

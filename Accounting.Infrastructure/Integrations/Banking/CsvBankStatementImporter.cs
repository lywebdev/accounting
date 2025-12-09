using System.Globalization;
using Accounting.Core.Constants;
using Accounting.Core.Entities;
using Accounting.Core.Interfaces.Integrations;
using Accounting.Core.ValueObjects;
using CsvHelper;

namespace Accounting.Infrastructure.Integrations.Banking;

public class CsvBankStatementImporter : IBankStatementImporter
{
    public async Task<IReadOnlyList<BankTransaction>> ParseAsync(Stream stream, CancellationToken cancellationToken = default)
    {
        using var reader = new StreamReader(stream, leaveOpen: true);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        var transactions = new List<BankTransaction>();

        while (await csv.ReadAsync())
        {
            cancellationToken.ThrowIfCancellationRequested();
            var dateValue = csv.GetField("Date");
            if (!DateOnly.TryParse(dateValue, CultureInfo.InvariantCulture, DateTimeStyles.None, out var bookingDate))
            {
                continue;
            }

            var counterparty = csv.TryGetField("Counterparty", out string? partner) ? partner ?? string.Empty : string.Empty;
            var reference = csv.TryGetField("Reference", out string? refValue) ? refValue ?? string.Empty : string.Empty;
            var currency = csv.TryGetField("Currency", out string? currencyValue) ? currencyValue : CurrencyCodes.Euro;
            var amountText = csv.TryGetField("Amount", out string? amountValue) ? amountValue : "0";
            if (!decimal.TryParse(amountText, NumberStyles.Number, CultureInfo.InvariantCulture, out var amount))
            {
                amount = 0m;
            }

            var money = new Money(amount, string.IsNullOrWhiteSpace(currency) ? CurrencyCodes.Euro : currency);
            transactions.Add(new BankTransaction(bookingDate, counterparty, reference, money));
        }

        return transactions;
    }
}

using Accounting.Core.Entities;
using Accounting.Core.Enums;
using Accounting.Core.ValueObjects;
using Accounting.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Accounting.Infrastructure.Seeding;

public class DatabaseSeeder(AccountingDbContext dbContext, ILogger<DatabaseSeeder> logger) : IDatabaseSeeder
{
    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Applying migrations...");
        await dbContext.Database.MigrateAsync(cancellationToken);

        await SeedAccountsAsync(cancellationToken);
        await SeedSampleInvoiceAsync(cancellationToken);

        logger.LogInformation("Database seeding completed.");
    }

    private async Task SeedAccountsAsync(CancellationToken cancellationToken)
    {
        if (await dbContext.Accounts.AnyAsync(cancellationToken))
        {
            return;
        }

        var accounts = new[]
        {
            new Account("1000", "Main bank account", AccountCategory.Assets, "Default operating account"),
            new Account("1100", "Petty cash", AccountCategory.Assets),
            new Account("1300", "Accounts receivable", AccountCategory.Assets),
            new Account("1400", "VAT receivable", AccountCategory.Assets),
            new Account("1600", "Accounts payable", AccountCategory.Liabilities),
            new Account("2000", "Share capital", AccountCategory.Equity),
            new Account("4000", "Consulting revenue", AccountCategory.Revenue),
            new Account("5000", "Office expenses", AccountCategory.Expense)
        };

        await dbContext.Accounts.AddRangeAsync(accounts, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Seeded default chart of accounts ({Count} accounts).", accounts.Length);
    }

    private async Task SeedSampleInvoiceAsync(CancellationToken cancellationToken)
    {
        if (await dbContext.Invoices.AnyAsync(cancellationToken))
        {
            return;
        }

        var revenueAccount = await dbContext.Accounts.AsNoTracking().FirstAsync(a => a.Number == "4000", cancellationToken);

        var invoice = new Invoice(InvoiceType.Sales, "INV-0001", "Contoso BV", DateOnly.FromDateTime(DateTime.UtcNow), DateOnly.FromDateTime(DateTime.UtcNow.AddDays(30)));
        invoice.AddLine(new InvoiceLine("Implementation services", 8, new Money(175m, "EUR"), 21m, revenueAccount.Id));
        invoice.MarkPosted();

        await dbContext.Invoices.AddAsync(invoice, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Seeded sample invoice {InvoiceNumber}.", invoice.Number);
    }
}

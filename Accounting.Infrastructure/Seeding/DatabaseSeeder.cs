using Accounting.Core.Constants;
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
        await SeedSampleInvoicesAsync(cancellationToken);
        await SeedJournalEntriesAsync(cancellationToken);
        await SeedBankTransactionsAsync(cancellationToken);

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

    private async Task SeedSampleInvoicesAsync(CancellationToken cancellationToken)
    {
        if (await dbContext.Invoices.AnyAsync(cancellationToken))
        {
            return;
        }

        var revenueAccounts = await dbContext.Accounts
            .Where(a => a.Category == AccountCategory.Revenue)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var consultingAccount = revenueAccounts.First(a => a.Number == "4000");
        var servicesAccount = revenueAccounts.FirstOrDefault(a => a.Number == "4100") ?? consultingAccount;

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var invoices = new List<Invoice>
        {
            CreateInvoice("INV-1001", "Contoso BV", today.AddDays(-20), today.AddDays(10), consultingAccount.Id, 6, 220m, 21m, posted: true, workflow: InvoiceWorkflowStatus.Sent),
            CreateInvoice("INV-1002", "Northwind AG", today.AddDays(-45), today.AddDays(-15), consultingAccount.Id, 10, 150m, 21m, posted: true, workflow: InvoiceWorkflowStatus.Overdue),
            CreateInvoice("INV-1003", "Fabrikam IT", today.AddDays(-15), today.AddDays(15), servicesAccount.Id, 4, 500m, 0m, posted: false, workflow: InvoiceWorkflowStatus.Draft),
            CreateInvoice("INV-1004", "Tailwind Traders", today.AddDays(-60), today.AddDays(-30), consultingAccount.Id, 12, 180m, 21m, posted: true, workflow: InvoiceWorkflowStatus.Paid)
        };

        await dbContext.Invoices.AddRangeAsync(invoices, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Seeded {Count} sample invoices.", invoices.Count);

        static Invoice CreateInvoice(string number, string counterparty, DateOnly issueDate, DateOnly dueDate, Guid revenueAccountId, int qty, decimal price, decimal vatRate, bool posted, InvoiceWorkflowStatus workflow)
        {
            var invoice = new Invoice(InvoiceType.Sales, number, counterparty, issueDate, dueDate);
            invoice.AddLine(new InvoiceLine("Professional services", qty, new Money(price, CurrencyCodes.Euro), vatRate, revenueAccountId));
            if (posted)
            {
                invoice.MarkPosted();
            }

            if (workflow == InvoiceWorkflowStatus.Sent)
            {
                invoice.MarkSent(DateTimeOffset.UtcNow.AddDays(-5));
            }
            else if (workflow == InvoiceWorkflowStatus.Overdue)
            {
                invoice.MarkSent(DateTimeOffset.UtcNow.AddDays(-20));
                invoice.RefreshWorkflowStatus(DateOnly.FromDateTime(DateTime.UtcNow));
            }
            else if (workflow == InvoiceWorkflowStatus.Paid)
            {
                invoice.MarkSent(DateTimeOffset.UtcNow.AddMonths(-2));
                invoice.RegisterPayment(DateTimeOffset.UtcNow.AddMonths(-1));
            }

            return invoice;
        }
    }

    private async Task SeedJournalEntriesAsync(CancellationToken cancellationToken)
    {
        if (await dbContext.JournalEntries.AnyAsync(cancellationToken))
        {
            return;
        }

        var accounts = await dbContext.Accounts.AsNoTracking().ToDictionaryAsync(a => a.Number, cancellationToken);
        var entries = new List<JournalEntry>();

        var payroll = new JournalEntry("JE-2001", DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-25)), "Monthly payroll");
        payroll.AddLine(new JournalEntryLine(accounts["5000"].Id, 4500m, 0m, "Salary expense"));
        payroll.AddLine(new JournalEntryLine(accounts["1600"].Id, 0m, 4500m, "Wages payable"));
        payroll.MarkPosted();
        entries.Add(payroll);

        var rent = new JournalEntry("JE-2002", DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-5)), "Office rent");
        rent.AddLine(new JournalEntryLine(accounts["5000"].Id, 2200m, 0m, "Rent expense"));
        rent.AddLine(new JournalEntryLine(accounts["1000"].Id, 0m, 2200m, "Bank payment"));
        rent.MarkPosted();
        entries.Add(rent);

        await dbContext.JournalEntries.AddRangeAsync(entries, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Seeded {Count} journal entries.", entries.Count);
    }

    private async Task SeedBankTransactionsAsync(CancellationToken cancellationToken)
    {
        if (await dbContext.BankTransactions.AnyAsync(cancellationToken))
        {
            return;
        }

        var invoices = await dbContext.Invoices.AsNoTracking().ToListAsync(cancellationToken);
        if (invoices.Count == 0)
        {
            return;
        }

        var transactions = new List<BankTransaction>
        {
            new(DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-2)), "Contoso BV", "Payment INV-1001", new Money(1600m, CurrencyCodes.Euro)),
            new(DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-18)), "Northwind AG", "Partial payment INV-1002", new Money(1000m, CurrencyCodes.Euro)),
            new(DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)), "Local supplier", "Office supplies", new Money(-320m, CurrencyCodes.Euro))
        };

        var contosoInvoice = invoices.FirstOrDefault(i => i.Number == "INV-1001");
        var northwindInvoice = invoices.FirstOrDefault(i => i.Number == "INV-1002");
        if (contosoInvoice is not null)
        {
            transactions[0].LinkToInvoice(contosoInvoice.Id);
        }

        if (northwindInvoice is not null)
        {
            transactions[1].LinkToInvoice(northwindInvoice.Id);
        }

        await dbContext.BankTransactions.AddRangeAsync(transactions, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Seeded {Count} bank transactions.", transactions.Count);
    }
}


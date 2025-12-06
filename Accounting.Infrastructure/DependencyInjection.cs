using System.IO;
using Accounting.Core.Interfaces.Integrations;
using Accounting.Core.Interfaces.Repositories;
using Accounting.Infrastructure.Integrations.Banking;
using Accounting.Infrastructure.Integrations.Tax;
using Accounting.Infrastructure.Persistence;
using Accounting.Infrastructure.Repositories;
using Accounting.Infrastructure.Seeding;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Accounting.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var databaseProvider = configuration["Database:Provider"] ?? "Sqlite";
        var sqliteConnection = configuration.GetConnectionString("Sqlite") ?? "Data Source=app_data/accounting.db";
        var sqlServerConnection = configuration.GetConnectionString("SqlServer") ??
                                  "Server=(localdb)\\MSSQLLocalDB;Database=AccountingApp;Trusted_Connection=True;MultipleActiveResultSets=true";

        services.AddDbContext<AccountingDbContext>(options =>
        {
            if (databaseProvider.Equals("SqlServer", StringComparison.OrdinalIgnoreCase))
            {
                options.UseSqlServer(sqlServerConnection);
            }
            else
            {
                var sqliteBuilder = new SqliteConnectionStringBuilder(sqliteConnection);
                if (!Path.IsPathRooted(sqliteBuilder.DataSource))
                {
                    sqliteBuilder.DataSource = Path.Combine(AppContext.BaseDirectory, sqliteBuilder.DataSource);
                }

                var directory = Path.GetDirectoryName(sqliteBuilder.DataSource);
                if (!string.IsNullOrEmpty(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                options.UseSqlite(sqliteBuilder.ConnectionString);
            }
        });

        services.AddScoped<IAccountRepository, AccountRepository>();
        services.AddScoped<IJournalEntryRepository, JournalEntryRepository>();
        services.AddScoped<IInvoiceRepository, InvoiceRepository>();
        services.AddScoped<IBankTransactionRepository, BankTransactionRepository>();
        services.AddScoped<ITaxDeclarationRepository, TaxDeclarationRepository>();
        services.AddScoped<IDatabaseSeeder, DatabaseSeeder>();

        services.AddSingleton<IBankFeedClient, FakeBankingApiClient>();
        services.AddSingleton<IBankStatementImporter, CsvBankStatementImporter>();
        services.AddSingleton<ITaxAuthorityClient, FakeTaxAuthorityApiClient>();

        return services;
    }
}

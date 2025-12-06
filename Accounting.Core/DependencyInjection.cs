using Accounting.Core.Entities;
using Accounting.Core.Interfaces.Services;
using Accounting.Core.Services;
using Accounting.Core.Validators;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Accounting.Core;

public static class DependencyInjection
{
    public static IServiceCollection AddCoreLayer(this IServiceCollection services)
    {
        services.AddScoped<IValidator<Account>, AccountValidator>();
        services.AddScoped<IValidator<JournalEntry>, JournalEntryValidator>();
        services.AddScoped<IValidator<Invoice>, InvoiceValidator>();

        services.AddScoped<IChartOfAccountsService, ChartOfAccountsService>();
        services.AddScoped<IJournalService, JournalService>();
        services.AddScoped<IInvoiceService, InvoiceService>();
        services.AddScoped<IBankingService, BankingService>();
        services.AddScoped<ITaxService, TaxService>();
        services.AddScoped<IReportingService, ReportingService>();

        return services;
    }
}

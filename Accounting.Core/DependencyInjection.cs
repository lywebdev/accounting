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

        services.AddScoped<IChartOfAccountsQueryService, ChartOfAccountsService>();
        services.AddScoped<IChartOfAccountsCommandService, ChartOfAccountsService>();
        services.AddScoped<IJournalQueryService, JournalService>();
        services.AddScoped<IJournalCommandService, JournalService>();
        services.AddScoped<IInvoiceQueryService, InvoiceService>();
        services.AddScoped<IInvoiceCommandService, InvoiceService>();
        services.AddScoped<IBankAutoMatchService, BankAutoMatchService>();
        services.AddScoped<IBankingService, BankingService>();
        services.AddScoped<ITaxService, TaxService>();
        services.AddScoped<IReportingService, ReportingService>();

        return services;
    }
}

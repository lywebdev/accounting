using System.Globalization;
using System.IO;
using Accounting.Core;
using Accounting.Core.Options;
using Accounting.Infrastructure;
using Accounting.Infrastructure.Seeding;
using Accounting.Web.Api.Accounts;
using Accounting.Web.Api.Banking;
using Accounting.Web.Api.Invoices;
using Accounting.Web.Api.JournalEntries;
using Accounting.Web.Api.Reporting;
using Accounting.Web.Api.Tax;
using Accounting.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;
using MudBlazor.Services;
using Serilog.Extensions.Logging.File;

namespace Accounting.Web;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var runSeeder = args.Any(a => a.Equals("--seed", StringComparison.OrdinalIgnoreCase) || a.Equals("seed", StringComparison.OrdinalIgnoreCase));

        var builder = WebApplication.CreateBuilder(args);

        var logsDirectory = Path.Combine(builder.Environment.ContentRootPath, "Logs");
        Directory.CreateDirectory(logsDirectory);
        builder.Logging.AddFile(Path.Combine(logsDirectory, "accountingapp-.log"));

        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents();

        builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
        builder.Services.AddMudServices();
        builder.Services.Configure<BankingSettings>(builder.Configuration.GetSection("Banking"));
        builder.Services.AddScoped(sp =>
        {
            var navigation = sp.GetRequiredService<NavigationManager>();
            return new HttpClient { BaseAddress = new Uri(navigation.BaseUri) };
        });

        var serviceCollection = builder.Services.AddCoreLayer();
        serviceCollection.AddInfrastructure(builder.Configuration);

        var supportedCultures = builder.Configuration.GetSection("Localization:SupportedCultures").Get<string[]>() ?? new[] { "en", "nl" };
        var defaultCulture = builder.Configuration.GetValue("Localization:DefaultCulture", "en")!;

        builder.Services.Configure<RequestLocalizationOptions>(options =>
        {
            var cultures = supportedCultures.Select(c => new CultureInfo(c)).ToArray();
            options.SupportedCultures = cultures;
            options.SupportedUICultures = cultures;
            options.DefaultRequestCulture = new RequestCulture(defaultCulture);
            options.RequestCultureProviders = new List<IRequestCultureProvider>
            {
                new QueryStringRequestCultureProvider(),
                new CookieRequestCultureProvider()
            };
        });

        var app = builder.Build();

        if (runSeeder)
        {
            using var scope = app.Services.CreateScope();
            var seeder = scope.ServiceProvider.GetRequiredService<IDatabaseSeeder>();
            await seeder.SeedAsync();
            return;
        }

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        var localizationOptions = app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value;
        app.UseRequestLocalization(localizationOptions);
        app.UseAntiforgery();

        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();

        app.MapAccountEndpoints();
        app.MapJournalEntryEndpoints();
        app.MapInvoiceEndpoints();
        app.MapBankingEndpoints();
        app.MapReportingEndpoints();
        app.MapTaxEndpoints();

        await app.RunAsync();
    }
}

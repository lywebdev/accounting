# Accounting
Accounting is a clean-architecture bookkeeping playground built with .NET 8, Razor Components, MudBlazor, and EF Core. It ships with a chart of accounts, invoices, banking, VAT tools, and reporting so you can explore the full flow end-to-end or extend it for production scenarios.

## Prerequisites
- .NET SDK 8.0+
- `dotnet-ef` (`dotnet tool update --global dotnet-ef`)
- SQLite (default) or SQL Server (optional switch via configuration)

## Getting Started (SQLite)
1. Restore dependencies  
   `dotnet restore`
2. Apply migrations to create/update `Accounting.Web/app_data/accounting.db`  
   `dotnet ef database update --project Accounting.Infrastructure/Accounting.Infrastructure.csproj --startup-project Accounting.Web/Accounting.Web.csproj`
3. Seed demo data (recommended for first run)  
   `dotnet run --project Accounting.Web -- seed`
4. Launch the app  
   `dotnet run --project Accounting.Web/Accounting.Web.csproj`
5. Browse to the URL shown in the console (`https://localhost:5001` by default). The UI loads in English; you can switch to Dutch, Russian, or Ukrainian via the language menu or the `?culture=` query string.

## Local Development Tasks
- **Add a migration**  
  `dotnet ef migrations add <MigrationName> --project Accounting.Infrastructure/Accounting.Infrastructure.csproj --startup-project Accounting.Web/Accounting.Web.csproj`
- **Apply migrations after schema changes**  
  `dotnet ef database update --project Accounting.Infrastructure/Accounting.Infrastructure.csproj --startup-project Accounting.Web/Accounting.Web.csproj`
- **Reseed demo data** (safe after migrations or whenever you need fresh fixtures)  
  `dotnet run --project Accounting.Web -- seed`
- **Reset SQLite**  
  Delete `Accounting.Web/app_data/accounting.db`, then rerun the migration and seeding commands.

## Switching to SQL Server
1. Configure `Database:Provider=SqlServer` and set `ConnectionStrings:SqlServer` in `appsettings.Development.json` or user secrets.
2. Run the same `dotnet ef database update` command; EF will target SQL Server automatically.
3. (Optional) Seed demo data: `dotnet run --project Accounting.Web -- seed`.

## Localization
- Resource files live under `Accounting.Web/Resources` with the shared marker `Accounting.Web.Localization.SharedResources`.
- Supported UI cultures: `en` (default), `nl`, `ru`, `uk`. Update `appsettings.json` or `appsettings.*.json` if you add more.
- The `LanguageSwitcher` component sets a cookie plus `culture/ui-culture` query parameters so URLs remain shareable per language.

## Helpful Paths
- Sample CSV for banking imports: `Accounting.Web/wwwroot/samples/banking-sample.csv`
- Application logs: `Accounting.Web/Logs`
- Database file (SQLite): `Accounting.Web/app_data/accounting.db`

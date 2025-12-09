# Accounting
Accounting is a full-featured bookkeeping workspace built with .NET 8, Razor Components, and MudBlazor. It includes a chart of accounts, invoices, journal entries, tax tools, and a banking module, so it can serve as a demo environment or the foundation of a production-grade app.

## Requirements
- .NET SDK 8.0 (build, migrations, and hosting).
- SQLite is the default database; the file lives at `Accounting.Web/app_data/accounting.db`.
- `dotnet-ef` CLI tool is required for manual migration management.

## Quick Start (SQLite)
1. Install or update `dotnet-ef` if needed:
   ```powershell
   dotnet tool update --global dotnet-ef --version 8.0.10
   ```
2. Restore dependencies:
   ```powershell
   dotnet restore
   ```
3. Apply migrations (creates the SQLite database file):
   ```powershell
   dotnet ef database update --project Accounting.Infrastructure/Accounting.Infrastructure.csproj --startup-project Accounting.Web/Accounting.Web.csproj
   ```
4. Seed demo data (optional but recommended so the UI has invoices, accounts, etc.):
   ```powershell
   dotnet run --project Accounting.Web -- seed
   ```
5. Start the web host:
   ```powershell
   dotnet run --project Accounting.Web/Accounting.Web.csproj
   ```
6. Browse to `https://localhost:5001` (or the port shown in the console). English is the default UI culture; other languages are available via the built-in localization menu or `?culture=xx`.

## Database Operations
- Update database to the latest migration:
  ```powershell
  dotnet ef database update --project Accounting.Infrastructure/Accounting.Infrastructure.csproj --startup-project Accounting.Web/Accounting.Web.csproj
  ```
- Add a new migration:
  ```powershell
  dotnet ef migrations add <MigrationName> --project Accounting.Infrastructure/Accounting.Infrastructure.csproj --startup-project Accounting.Web/Accounting.Web.csproj
  ```
- Re-run demo seeding after migrations:
  ```powershell
  dotnet run --project Accounting.Web -- seed
  ```
- Reset SQLite completely: delete `Accounting.Web/app_data/accounting.db`, then repeat the migration and seeding commands.

## Switching to SQL Server or Azure SQL
1. Set the following in `appsettings.*` or environment variables:
   - `Database:Provider=SqlServer`
   - `ConnectionStrings:SqlServer=<your connection string>`
2. Run `dotnet ef database update` against the SQL Server connection.
3. Seed data by running `dotnet run --project Accounting.Web -- seed` if you want the same demo content.

## Useful References
- Sample banking CSV is available at `wwwroot/samples/banking-sample.csv` for quick imports.
- Application logs are written to `Accounting.Web/Logs`.
- All localization resources live under `Accounting.Web/Resources`; you can switch cultures via the UI or by appending `?culture=xx` to any page.

# accounting
A full-featured accounting system built with C# (.NET 8) and MudBlazor, showcasing production-grade architecture, reporting, imports/exports, and integrations.

## Getting started

### Quick demo (SQLite by default)
1. Restore tooling if `dotnet-ef` is missing:
   ```powershell
   dotnet tool update --global dotnet-ef --version 8.0.10
   ```
2. Apply the initial migration (creates `Accounting.Web/app_data/accounting.db`):
   ```powershell
   dotnet ef database update --project Accounting.Infrastructure/Accounting.Infrastructure.csproj --startup-project Accounting.Web/Accounting.Web.csproj
   ```
3. Run the web host:
   ```powershell
   dotnet run --project Accounting.Web/Accounting.Web.csproj
   ```

### Migration / seeding helpers
Use the built-in EF Core commands:

```bash
# Apply or update migrations (run from repo root)
dotnet ef database update --project Accounting.Infrastructure/Accounting.Infrastructure.csproj --startup-project Accounting.Web/Accounting.Web.csproj

# Run only the demo seeder (after migrations)
dotnet run --project Accounting.Web -- seed

# Full reset (SQLite): delete app_data/accounting.db, then run the two commands above
```

The seed routine populates the default chart of accounts plus a sample sales invoice.

### Switching to SQL Server / Azure SQL
- Set `Database:Provider` to `SqlServer` (environment variable or `appsettings.Production.json`).
- Provide a valid `ConnectionStrings:SqlServer`.
- Re-run `dotnet ef database update ...` against the target database.

This dual profile keeps the repo easy to clone/run (SQLite file) while staying production ready (SQL Server/Azure SQL).

## UX helpers
- **Chart of Accounts** displays status hints (active vs inactive) so you know when a ledger can be used in transactions.
- **Journal & Invoice dialogs** run fullscreen, include inline guides, templates, and payment-term selectors. Use the Preview button in the invoice dialog to open a printable HTML summary.
- **Banking workspace** provides inline helper text, search/amount filters, and an empty-state hint that links to the sync/import actions.
- **Sample banking data** is available via `wwwroot/samples/banking-sample.csv` (the UI links to it from the empty state).
- **Reporting filters** show helper text next to each input, explaining which tabs rely on the selected criteria.

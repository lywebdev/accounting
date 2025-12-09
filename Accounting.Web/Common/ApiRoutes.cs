namespace Accounting.Web.Common;

public static class ApiRoutes
{
    public static class Banking
    {
        public const string Transactions = "api/banking/transactions";
        public const string Import = "api/banking/transactions/import";
        public const string Sync = "api/banking/transactions/sync";
        public const string AutoMatch = "api/banking/transactions/auto-match";
    }

    public static class Invoices
    {
        public const string Base = "api/invoices";
        public static string Pdf(Guid id) => $"{Base}/{id}/pdf";
        public static string Post(Guid id) => $"{Base}/{id}/post";
    }

    public static class Reporting
    {
        public const string ProfitAndLoss = "api/reporting/profit-and-loss";
        public const string BalanceSheet = "api/reporting/balance-sheet";
        public const string TrialBalance = "api/reporting/trial-balance";
        public const string GeneralLedger = "api/reporting/general-ledger";
    }

    public static class Tax
    {
        public const string Declaration = "api/tax/declaration";
        public const string SubmitDeclaration = "api/tax/declaration/submit";
        public static string DeclarationsByYear(int year) => $"api/tax/declarations/{year}";
        public const string ValidateVat = "api/tax/validate";
    }
}

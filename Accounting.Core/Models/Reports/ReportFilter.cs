using Accounting.Core.Constants;
using Accounting.Core.Enums;

namespace Accounting.Core.Models.Reports;

public record ReportFilter(
    DateOnly From,
    DateOnly To,
    AccountCategory? Category = null,
    Guid? AccountId = null,
    string? Company = null,
    string Currency = CurrencyCodes.Euro);

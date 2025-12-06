namespace Accounting.Core.Models.Reports;

public record ReportResult<T>(IReadOnlyList<T> Rows, DateTimeOffset GeneratedAt, string Currency);

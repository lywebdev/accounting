using Accounting.Core.Entities;
using Accounting.Core.Enums;

namespace Accounting.Web.Api.Tax;

public sealed record TaxDeclarationDto(
    Guid Id,
    int Year,
    int Period,
    decimal VatPayable,
    decimal VatReceivable,
    decimal NetAmount,
    TaxDeclarationStatus Status,
    DateTimeOffset? SubmittedAt);

public sealed record VatValidationResponse(bool IsValid);

public sealed record SubmitTaxDeclarationRequest(int Year, int Period);

public sealed record VatValidationRequest(string VatNumber);

internal static class TaxDeclarationMappings
{
    public static TaxDeclarationDto ToDto(this TaxDeclaration declaration) =>
        new(declaration.Id, declaration.Year, declaration.Period, declaration.VatPayable, declaration.VatReceivable, declaration.VatPayable - declaration.VatReceivable, declaration.Status, declaration.SubmittedAt);
}

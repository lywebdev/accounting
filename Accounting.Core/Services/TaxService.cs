using Accounting.Core.Constants;
using Accounting.Core.Entities;
using Accounting.Core.Enums;
using Accounting.Core.Interfaces.Integrations;
using Accounting.Core.Interfaces.Repositories;
using Accounting.Core.Interfaces.Services;

namespace Accounting.Core.Services;

public class TaxService(
    ITaxDeclarationRepository declarationRepository,
    IInvoiceRepository invoiceRepository,
    ITaxAuthorityClient taxAuthorityClient) : ITaxService
{
    public async Task<TaxDeclaration> CalculateAsync(int year, int period, CancellationToken cancellationToken = default)
    {
        var declaration = await declarationRepository.GetByPeriodAsync(year, period, cancellationToken) ?? new TaxDeclaration(year, period);
        var (from, to) = GetQuarterRange(year, period);
        var invoices = await invoiceRepository.GetAsync(null, from, to, searchTerm: null, cancellationToken);

        var salesVat = invoices.Where(i => i.Type == InvoiceType.Sales).Sum(i => i.TotalVat(CurrencyCodes.Euro).Amount);
        var purchaseVat = invoices.Where(i => i.Type == InvoiceType.Purchase).Sum(i => i.TotalVat(CurrencyCodes.Euro).Amount);
        declaration.SetFigures(salesVat, purchaseVat);
        await declarationRepository.SaveAsync(declaration, cancellationToken);
        return declaration;
    }

    public async Task<TaxDeclaration> SubmitAsync(int year, int period, CancellationToken cancellationToken = default)
    {
        var declaration = await CalculateAsync(year, period, cancellationToken);
        var submissionId = await taxAuthorityClient.SubmitDeclarationAsync(year, period);
        var status = submissionId.Contains("REJECTED", StringComparison.OrdinalIgnoreCase)
            ? TaxDeclarationStatus.Rejected
            : TaxDeclarationStatus.Accepted;
        declaration.MarkSubmitted(status);
        await declarationRepository.SaveAsync(declaration, cancellationToken);
        return declaration;
    }

    public async Task<IReadOnlyList<TaxDeclaration>> GetYearAsync(int year, CancellationToken cancellationToken = default)
        => await declarationRepository.GetByYearAsync(year, cancellationToken);

    public Task<bool> ValidateVatNumberAsync(string vatNumber, CancellationToken cancellationToken = default)
        => taxAuthorityClient.ValidateVatAsync(vatNumber);

    private static (DateOnly from, DateOnly to) GetQuarterRange(int year, int period)
    {
        var startMonth = ((period - 1) * 3) + 1;
        var from = new DateOnly(year, startMonth, 1);
        var to = from.AddMonths(3).AddDays(-1);
        return (from, to);
    }
}


using Accounting.Core.Entities;

namespace Accounting.Core.Interfaces.Services;

public interface IInvoiceDocumentService
{
    Task<byte[]> GeneratePdfAsync(Invoice invoice);
}

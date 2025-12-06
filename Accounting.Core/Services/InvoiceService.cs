using Accounting.Core.Entities;
using Accounting.Core.Enums;
using Accounting.Core.Interfaces.Repositories;
using Accounting.Core.Interfaces.Services;
using FluentValidation;

namespace Accounting.Core.Services;

public class InvoiceService(IInvoiceRepository repository, IValidator<Invoice> validator) : IInvoiceService
{
    public Task<IReadOnlyList<Invoice>> GetAsync(InvoiceType? type, DateOnly? from, DateOnly? to, CancellationToken cancellationToken = default)
        => repository.GetAsync(type, from, to, cancellationToken);

    public async Task<Invoice> CreateAsync(InvoiceType type, string number, string counterparty, DateOnly issueDate, DateOnly dueDate, IEnumerable<InvoiceLine> lines, CancellationToken cancellationToken = default)
    {
        var invoice = new Invoice(type, number, counterparty, issueDate, dueDate);
        foreach (var line in lines)
        {
            invoice.AddLine(line);
        }

        await validator.ValidateAndThrowAsync(invoice, cancellationToken);
        await repository.AddAsync(invoice, cancellationToken);
        return invoice;
    }

    public async Task<Invoice> PostAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var invoice = await repository.GetByIdAsync(id, cancellationToken) ?? throw new InvalidOperationException("Invoice not found");
        invoice.MarkPosted();
        await repository.UpdateAsync(invoice, cancellationToken);
        return invoice;
    }

    public Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        => repository.DeleteAsync(id, cancellationToken);
}

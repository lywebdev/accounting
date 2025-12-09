using Accounting.Core.Entities;
using Accounting.Core.Enums;
using Accounting.Core.Interfaces.Repositories;
using Accounting.Core.Interfaces.Services;
using FluentValidation;

namespace Accounting.Core.Services;

public class InvoiceService(IInvoiceRepository repository, IValidator<Invoice> validator) : IInvoiceQueryService, IInvoiceCommandService
{
    public async Task<IReadOnlyList<Invoice>> GetAsync(InvoiceType? type, DateOnly? from, DateOnly? to, string? searchTerm, CancellationToken cancellationToken = default)
    {
        var invoices = await repository.GetAsync(type, from, to, searchTerm, cancellationToken);
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        foreach (var invoice in invoices)
        {
            if (invoice.RefreshWorkflowStatus(today))
            {
                await repository.UpdateAsync(invoice, cancellationToken);
            }
        }

        return invoices;
    }

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

    public async Task<Invoice> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var invoice = await repository.GetByIdAsync(id, cancellationToken);
        return invoice ?? throw new InvalidOperationException("Invoice not found");
    }

    public async Task<Invoice> PostAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var invoice = await repository.GetByIdAsync(id, cancellationToken) ?? throw new InvalidOperationException("Invoice not found");
        invoice.MarkPosted();
        await repository.UpdateAsync(invoice, cancellationToken);
        return invoice;
    }

    public async Task<Invoice> MarkSentAsync(Guid id, DateOnly sentDate, CancellationToken cancellationToken = default)
    {
        var invoice = await repository.GetByIdAsync(id, cancellationToken) ?? throw new InvalidOperationException("Invoice not found");
        var sentAt = new DateTimeOffset(sentDate.ToDateTime(TimeOnly.MinValue), TimeSpan.Zero);
        invoice.MarkSent(sentAt);
        await repository.UpdateAsync(invoice, cancellationToken);
        return invoice;
    }

    public async Task<Invoice> RecordPaymentAsync(Guid id, DateOnly paymentDate, CancellationToken cancellationToken = default)
    {
        var invoice = await repository.GetByIdAsync(id, cancellationToken) ?? throw new InvalidOperationException("Invoice not found");
        var paidAt = new DateTimeOffset(paymentDate.ToDateTime(TimeOnly.MinValue), TimeSpan.Zero);
        invoice.RegisterPayment(paidAt);
        await repository.UpdateAsync(invoice, cancellationToken);
        return invoice;
    }

    public Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        => repository.DeleteAsync(id, cancellationToken);
}

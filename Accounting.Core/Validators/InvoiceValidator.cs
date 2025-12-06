using Accounting.Core.Entities;
using FluentValidation;

namespace Accounting.Core.Validators;

public class InvoiceValidator : AbstractValidator<Invoice>
{
    public InvoiceValidator()
    {
        RuleFor(i => i.Number).NotEmpty().MaximumLength(32);
        RuleFor(i => i.Counterparty).NotEmpty();
        RuleFor(i => i.Lines).NotEmpty();
    }
}

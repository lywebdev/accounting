using Accounting.Core.Entities;
using FluentValidation;

namespace Accounting.Core.Validators;

public class AccountValidator : AbstractValidator<Account>
{
    public AccountValidator()
    {
        RuleFor(a => a.Number).NotEmpty().MaximumLength(20);
        RuleFor(a => a.Name).NotEmpty().MaximumLength(128);
    }
}

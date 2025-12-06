using Accounting.Core.Entities;
using FluentValidation;

namespace Accounting.Core.Validators;

public class JournalEntryValidator : AbstractValidator<JournalEntry>
{
    public JournalEntryValidator()
    {
        RuleFor(j => j.Reference).NotEmpty();
        RuleFor(j => j.Lines).NotEmpty();
        RuleFor(j => j).Must(j => j.IsBalanced).WithMessage("Journal entry must be balanced.");
    }
}

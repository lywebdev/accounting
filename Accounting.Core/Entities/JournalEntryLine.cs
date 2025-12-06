namespace Accounting.Core.Entities;

public class JournalEntryLine
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid JournalEntryId { get; private set; }
    public Guid AccountId { get; private set; }
    public decimal Debit { get; private set; }
    public decimal Credit { get; private set; }
    public string Description { get; private set; }

    public JournalEntryLine(Guid accountId, decimal debit, decimal credit, string description)
    {
        if (debit < 0 || credit < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(debit), "Amounts must be positive.");
        }

        if (debit == 0 && credit == 0)
        {
            throw new ArgumentException("Debit or credit amount must be supplied.", nameof(debit));
        }

        AccountId = accountId;
        Debit = decimal.Round(debit, 2, MidpointRounding.AwayFromZero);
        Credit = decimal.Round(credit, 2, MidpointRounding.AwayFromZero);
        Description = description ?? string.Empty;
    }

    internal void AssignEntry(Guid entryId) => JournalEntryId = entryId;

    private JournalEntryLine()
    {
        Description = string.Empty;
    }
}

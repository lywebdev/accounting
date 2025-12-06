namespace Accounting.Core.Entities;

public class JournalEntry
{
    private readonly List<JournalEntryLine> _lines = new();

    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Reference { get; private set; }
    public DateOnly EntryDate { get; private set; }
    public string? Memo { get; private set; }
    public IReadOnlyCollection<JournalEntryLine> Lines => _lines.AsReadOnly();
    public bool IsPosted { get; private set; }

    public JournalEntry(string reference, DateOnly entryDate, string? memo = null)
    {
        Reference = reference ?? throw new ArgumentNullException(nameof(reference));
        EntryDate = entryDate;
        Memo = memo;
    }

    public void AddLine(JournalEntryLine line)
    {
        ArgumentNullException.ThrowIfNull(line);
        line.AssignEntry(Id);
        _lines.Add(line);
    }

    public void RemoveLine(Guid lineId)
    {
        var line = _lines.FirstOrDefault(l => l.Id == lineId);
        if (line is not null)
        {
            _lines.Remove(line);
        }
    }

    public decimal TotalDebit => _lines.Sum(l => l.Debit);
    public decimal TotalCredit => _lines.Sum(l => l.Credit);
    public bool IsBalanced => decimal.Round(TotalDebit, 2) == decimal.Round(TotalCredit, 2);

    public void MarkPosted()
    {
        if (!IsBalanced)
        {
            throw new InvalidOperationException("Cannot post an unbalanced journal entry.");
        }

        IsPosted = true;
    }

    private JournalEntry()
    {
        Reference = string.Empty;
    }
}

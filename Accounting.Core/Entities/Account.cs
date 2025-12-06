using Accounting.Core.Enums;

namespace Accounting.Core.Entities;

public class Account
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Number { get; private set; }
    public string Name { get; private set; }
    public AccountCategory Category { get; private set; }
    public bool IsActive { get; private set; } = true;
    public string? Description { get; private set; }
    public DateTimeOffset CreatedAt { get; } = DateTimeOffset.UtcNow;

    public Account(string number, string name, AccountCategory category, string? description = null)
    {
        Number = number ?? throw new ArgumentNullException(nameof(number));
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Category = category;
        Description = description;
    }

    public void Update(string name, AccountCategory category, string? description)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Category = category;
        Description = description;
    }

    public void ChangeNumber(string number)
    {
        Number = number ?? throw new ArgumentNullException(nameof(number));
    }

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;

    private Account()
    {
        Number = string.Empty;
        Name = string.Empty;
    }
}

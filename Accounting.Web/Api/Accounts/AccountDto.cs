using Accounting.Core.Entities;
using Accounting.Core.Enums;

namespace Accounting.Web.Api.Accounts;

public sealed record AccountDto(
    Guid Id,
    string Number,
    string Name,
    AccountCategory Category,
    string? Description,
    bool IsActive);

internal static class AccountMappings
{
    public static AccountDto ToDto(this Account account) =>
        new(account.Id, account.Number, account.Name, account.Category, account.Description, account.IsActive);
}

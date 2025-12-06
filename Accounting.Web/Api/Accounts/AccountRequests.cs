using System.ComponentModel.DataAnnotations;
using Accounting.Core.Enums;

namespace Accounting.Web.Api.Accounts;

public sealed record CreateAccountRequest(
    [Required, StringLength(20)] string Number,
    [Required, StringLength(128)] string Name,
    [Required] AccountCategory Category,
    [StringLength(256)] string? Description);

public sealed record UpdateAccountRequest(
    [Required, StringLength(128)] string Name,
    [Required] AccountCategory Category,
    [StringLength(256)] string? Description);

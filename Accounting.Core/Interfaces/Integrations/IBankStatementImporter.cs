using Accounting.Core.Entities;

namespace Accounting.Core.Interfaces.Integrations;

public interface IBankStatementImporter
{
    Task<IReadOnlyList<BankTransaction>> ParseAsync(Stream stream, CancellationToken cancellationToken = default);
}

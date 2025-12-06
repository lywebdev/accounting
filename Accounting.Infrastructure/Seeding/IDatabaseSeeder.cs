using System.Threading;
using System.Threading.Tasks;

namespace Accounting.Infrastructure.Seeding;

public interface IDatabaseSeeder
{
    Task SeedAsync(CancellationToken cancellationToken = default);
}

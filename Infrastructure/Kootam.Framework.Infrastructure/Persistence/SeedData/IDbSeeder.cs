using Kootam.Framework.Infrastructure.Persistence.DbContexts;

namespace Kootam.Framework.Infrastructure.Persistence.SeedData;

public interface IDbSeeder<TContext>
{
    Task SeedAsync(TContext context,CancellationToken cancellationToken = default);
}
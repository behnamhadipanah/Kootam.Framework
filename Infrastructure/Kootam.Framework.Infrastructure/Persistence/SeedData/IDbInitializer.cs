using Kootam.Framework.Infrastructure.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Kootam.Framework.Infrastructure.Persistence.SeedData;

public interface IDbInitializer
{
    Task InitializeAsync(CancellationToken cancellationToken = default);

}

public sealed class DbInitializer<TContext>(
    IServiceScopeFactory scopeFactory,
    ILogger<DbInitializer<TContext>> logger,
    IEnumerable<IDbSeeder<TContext>> seeders)
    : IDbInitializer
    where TContext : BaseCommandDbContext<TContext>
{
    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        await using var scope = scopeFactory.CreateAsyncScope();

        var context = scope.ServiceProvider.GetRequiredService<TContext>();

        try
        {
            logger.LogInformation("Applying migrations for {Context}.",
                typeof(TContext).Name);

            await context.Database.MigrateAsync(cancellationToken);

            logger.LogInformation("Database migrated.");

            foreach (var seeder in seeders)
            {
                await seeder.SeedAsync(context,cancellationToken);
            }

            logger.LogInformation("Database seeded.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Database initialization failed for {Context}.",
                typeof(TContext).Name);

            throw;
        }
    }
}
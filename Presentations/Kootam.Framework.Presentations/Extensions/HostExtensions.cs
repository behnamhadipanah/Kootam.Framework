using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Net.Sockets;

namespace Kootam.Framework.Presentations.Extensions;

public static class HostExtensions
{
    public static async Task<IHost> MigrationDatabaseAsync<TContext>(this IHost host,
        Func<TContext, IServiceProvider, Task> seeder,
        int retry = 0) where TContext : DbContext
    {
        using var scope = host.Services.CreateScope();

        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<TContext>>();
        var context = services.GetRequiredService<TContext>();

        try
        {
            logger.LogInformation("Database migration started");

            await context.Database.MigrateAsync();

            await seeder(context, services);

            logger.LogInformation("Database migration completed");
        }
        catch (Exception ex) when (ex is SqlException || ex is IOException || ex is SocketException)
        {
            logger.LogError(ex, "Error while migrating database");

            if (retry < 50)
            {
                await Task.Delay(2000);

                return await host.MigrationDatabaseAsync<TContext>(
                    seeder,
                    retry + 1);
            }

            throw;
        }

        return host;
    }

    public static Task<IHost> MigrationDatabaseAsync<TContext>(this IHost host,int retry = 0) where TContext : DbContext
    {
        return host.MigrationDatabaseAsync<TContext>(
            seeder: (_, _) => Task.CompletedTask,
            retry: retry);
    }
}
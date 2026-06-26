using Kootam.Framework.Infrastructure.Persistence.DbContexts;
using Kootam.Framework.Infrastructure.Persistence.SeedData;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


namespace Kootam.Framework.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDatabaseInitializer<TContext>(
        this IServiceCollection services)
        where TContext : BaseCommandDbContext<TContext>
    {
        services.AddScoped<IDbInitializer, DbInitializer<TContext>>();

        return services;
    }

    public static async Task<IHost> InitializeDatabaseAsync(this IHost host)
    {
        using var scope = host.Services.CreateScope();

        var initializers = scope.ServiceProvider.GetServices<IDbInitializer>();

        foreach (var initializer in initializers)
        {
            await initializer.InitializeAsync();
        }

        return host;
    }

}
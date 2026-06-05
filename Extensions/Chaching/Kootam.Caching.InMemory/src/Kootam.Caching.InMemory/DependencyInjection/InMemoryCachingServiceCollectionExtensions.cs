using Kootam.Caching.InMemory.Adapter;
using Kootam.Caching.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Kootam.Caching.InMemory.DependencyInjection;

public static class InMemoryCachingServiceCollectionExtensions
{
    public static IServiceCollection AddInMemoryCaching(this IServiceCollection services)
    {
        services.AddSingleton<ICacheStore, InMemoryCacheAdapter>();

        return services;

    }
}
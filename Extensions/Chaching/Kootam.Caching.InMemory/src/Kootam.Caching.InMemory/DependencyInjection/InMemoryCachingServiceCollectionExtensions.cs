using Kootam.Extensions.Caching.Abstractions;
using Kootam.Extensions.Caching.InMemory.Adapter;
using Microsoft.Extensions.DependencyInjection;

namespace Kootam.Extensions.Caching.InMemory.DependencyInjection;

public static class InMemoryCachingServiceCollectionExtensions
{
    public static IServiceCollection AddInMemoryCaching(this IServiceCollection services)
    {
        services.AddSingleton<ICacheStore, InMemoryCacheAdapter>();

        return services;

    }
}
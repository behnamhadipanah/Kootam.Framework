using Kootam.Framework.AutoMap.Abstractions;
using Mapster;
using Microsoft.Extensions.DependencyInjection;

namespace Kootam.Framework.AutoMap.Mapster.DependencyInjection;

public static class MapsterCollectionExtensions
{
    public static IServiceCollection AddMapsterMapping(this IServiceCollection services)
    {
        services.AddMapster();
        services.AddScoped<IMapper, MapsterMapperAdapter>();

        return services;
    }
}

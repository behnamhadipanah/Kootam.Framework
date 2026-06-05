using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Kootam.AutoMap.Abstractions;

namespace Kootam.AutoMap.AutoMapper.DependencyInjection;

public static class AutoMapperCollectionExtensions
{
    public static IServiceCollection AddAutoMapperMapping(this IServiceCollection services, params Assembly[] assemblies)
    {
        services.AddAutoMapper(cfg => { }, assemblies);

        services.AddScoped<IMapper, AutoMapperAdapter>();

        return services;
    }

}

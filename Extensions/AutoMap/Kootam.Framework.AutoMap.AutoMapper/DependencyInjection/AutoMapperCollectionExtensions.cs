using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using IMapper = Kootam.Framework.AutoMap.Abstractions.IMapper;

namespace Kootam.Framework.AutoMap.AutoMapper.DependencyInjection;

public static class AutoMapperCollectionExtensions
{
    public static IServiceCollection AddKootamAutoMapper(this IServiceCollection services, params Assembly[] assemblies)
    {
        services.AddAutoMapper(cfg => { }, assemblies);

        services.AddScoped<IMapper, AutoMapperAdapter>();

        return services;
    }

}

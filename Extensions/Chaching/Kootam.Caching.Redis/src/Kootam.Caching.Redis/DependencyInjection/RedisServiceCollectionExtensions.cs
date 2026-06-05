using Kootam.Caching.Abstractions;
using Kootam.Caching.Redis.Adapter;
using Kootam.Caching.Redis.Configuration;
using Kootam.Caching.Redis.Context;
using Kootam.Caching.Redis.HealthChecks;
using Kootam.Caching.Redis.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Kootam.Caching.Redis.DependencyInjection;

/// <summary>
/// Extension methods for registering Redis caching services
/// </summary>
public static class RedisCachingServiceCollectionExtensions
{
    public static IServiceCollection AddRedisCaching(
        this IServiceCollection services,
        IConfiguration configuration,
        string sectionName = RedisDBConfigs.SectionName)
    {
        services.Configure<RedisDBConfigs>(configuration.GetSection(sectionName));
        AddCommonValidationAndCoreServices(services);
        return services;
    }

    public static IServiceCollection AddRedisCaching(
        this IServiceCollection services,
        Action<RedisDBConfigs> configureOptions)
    {
        services.Configure(configureOptions);
        AddCommonValidationAndCoreServices(services);
        return services;
    }

    public static IServiceCollection AddRedisCaching(
        this IServiceCollection services,
        string host,
        int port = 6379,
        int database = 0,
        string? password = null,
        string configName = "Default")
    {
        services.Configure<RedisDBConfigs>(options =>
        {
            options.Configs = new List<RedisDBConfigModel>
            {
                new RedisDBConfigModel
                {
                    Name = configName,
                    Host = host,
                    Port = port,
                    DBNumber = database,
                    Password = password
                }
            };
        });

        AddCommonValidationAndCoreServices(services);
        return services;
    }

    
    public static IServiceCollection AddRedisHealthChecks(
        this IServiceCollection services,
        string name = "redis",
        HealthStatus? failureStatus = null,
        IEnumerable<string>? tags = null,
        TimeSpan? timeout = null)
    {
        var healthCheckBuilder = services.AddHealthChecks();

        healthCheckBuilder.AddCheck<RedisHealthCheck>(
            name,
            failureStatus ?? HealthStatus.Unhealthy,
            tags ?? new[] { "ready", "cache", "redis" },
            timeout);

        return services;
    }
    private static void AddCommonValidationAndCoreServices(IServiceCollection services)
    {
        services
            .AddOptions<RedisDBConfigs>()
            .Validate(config =>
            {
                config.Validate();
                return true;
            }, "Redis configuration is invalid");

        RegisterCoreServices(services);
    }

    private static void RegisterCoreServices(IServiceCollection services)
    {
        services.AddSingleton<IRedisContext, RedisContext>();
        services.AddSingleton<IRedisRepository, RedisRepository>();
        services.AddSingleton<ICacheAdapter, RedisCacheAdapter>();
    }
}
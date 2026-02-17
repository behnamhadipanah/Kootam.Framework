using Kootam.Framework.Caching.Abstractions;
using Kootam.Framework.Caching.Redis.Adapter;
using Kootam.Framework.Caching.Redis.Configuration;
using Kootam.Framework.Caching.Redis.Context;
using Kootam.Framework.Caching.Redis.HealthChecks;
using Kootam.Framework.Caching.Redis.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Kootam.Framework.Caching.Redis.Extensions;

/// <summary>
/// Extension methods for registering Redis caching services
/// </summary>
public static class RedisCachingServiceCollectionExtensions
{
    /// <summary>
    /// Add Redis caching services from configuration (appsettings.json)
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="configuration">Configuration instance</param>
    /// <param name="sectionName">Configuration section name (default: "Redis")</param>
    /// <returns>Service collection for chaining</returns>
    public static IServiceCollection AddRedisCaching(
        this IServiceCollection services,
        IConfiguration configuration,
        string sectionName = RedisDBConfigs.SectionName)
    {
        services.Configure<RedisDBConfigs>(options =>
        {
            var section = configuration.GetSection(sectionName);
        });
        
        
        services.AddOptions<RedisDBConfigs>()
            .Validate(config =>
            {
                try
                {
                    config.Validate();
                    return true;
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(
                        $"Redis configuration validation failed: {ex.Message}", ex);
                }
            }, "Redis configuration is invalid");

        RegisterCoreServices(services);

        return services;
    }

    /// <summary>
    /// Add Redis caching services with configuration action
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="configureOptions">Configuration action</param>
    /// <returns>Service collection for chaining</returns>
    public static IServiceCollection AddRedisCaching(
        this IServiceCollection services,
        Action<RedisDBConfigs> configureOptions)
    {
        services.Configure(configureOptions);

        services.AddOptions<RedisDBConfigs>()
            .Validate(config =>
            {
                try
                {
                    config.Validate();
                    return true;
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(
                        $"Redis configuration validation failed: {ex.Message}", ex);
                }
            }, "Redis configuration is invalid");

        RegisterCoreServices(services);

        return services;
    }

    /// <summary>
    /// Add Redis caching services with inline configuration
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="host">Redis host</param>
    /// <param name="port">Redis port (default: 6379)</param>
    /// <param name="database">Database number (default: 0)</param>
    /// <param name="password">Password (optional)</param>
    /// <param name="configName">Configuration name (default: "Default")</param>
    /// <returns>Service collection for chaining</returns>
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

        // Validate configuration on startup
        services.AddOptions<RedisDBConfigs>()
            .Validate(config =>
            {
                try
                {
                    config.Validate();
                    return true;
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(
                        $"Redis configuration validation failed: {ex.Message}", ex);
                }
            }, "Redis configuration is invalid");

        RegisterCoreServices(services);

        return services;
    }

    /// <summary>
    /// Add health checks for Redis
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="name">Health check name (default: "redis")</param>
    /// <param name="failureStatus">Status to report on failure (default: Unhealthy)</param>
    /// <param name="tags">Health check tags (default: ready, cache, redis)</param>
    /// <param name="timeout">Timeout for health check (optional)</param>
    /// <returns>Service collection for chaining</returns>
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

    private static void RegisterCoreServices(IServiceCollection services)
    {
        services.AddSingleton<IRedisContext, RedisContext>();
        services.AddSingleton<IRedisRepository, RedisRepository>();
        services.AddSingleton<ICacheAdapter, RedisCacheAdapter>();
    }
}
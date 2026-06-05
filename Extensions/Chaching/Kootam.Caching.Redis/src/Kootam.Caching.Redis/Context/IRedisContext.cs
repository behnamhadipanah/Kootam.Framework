using StackExchange.Redis;

namespace Kootam.Caching.Redis.Context;

/// <summary>
/// Redis connection context for managing multiple Redis database connections
/// </summary>
public interface IRedisContext : IDisposable
{
    /// <summary>
    /// Get Redis database instance by configuration name
    /// </summary>
    /// <param name="configName">Configuration name (uses default if null)</param>
    /// <returns>Redis database instance</returns>
    IDatabase GetDatabase(string? configName = null);

    /// <summary>
    /// Get Redis server instance by configuration name
    /// </summary>
    /// <param name="configName">Configuration name (uses default if null)</param>
    /// <returns>Redis server instance</returns>
    IServer GetServer(string? configName = null);

    /// <summary>
    /// Get connection multiplexer by configuration name
    /// </summary>
    /// <param name="configName">Configuration name (uses default if null)</param>
    /// <returns>Connection multiplexer instance</returns>
    IConnectionMultiplexer GetConnection(string? configName = null);

    /// <summary>
    /// Check if a configuration exists
    /// </summary>
    /// <param name="configName">Configuration name</param>
    /// <returns>True if configuration exists</returns>
    bool ConfigExists(string configName);

    /// <summary>
    /// Get all available configuration names
    /// </summary>
    /// <returns>List of configuration names</returns>
    IReadOnlyList<string> GetConfigNames();
}



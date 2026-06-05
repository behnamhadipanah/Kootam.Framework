using Microsoft.Extensions.Logging;

namespace Kootam.Extensions.Caching.Redis.Repository;

/// <summary>
/// Redis advanced operations (Pub/Sub, Database management)
/// </summary>
public partial class RedisRepository
{
    /// <summary>
    /// Publish message to channel
    /// Example: PUBLISH notifications "New order received"
    /// </summary>
    public async Task<long> PublishAsync(string channel, string message, string? configName = null)
    {
        try
        {
            var db = _context.GetDatabase(configName);
            return await db.PublishAsync(channel, message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing to channel {Channel}", channel);
            throw;
        }
    }

    /// <summary>
    /// Get all keys matching pattern
    /// Example: KEYS user:*
    ///  Warning: This can be slow on large databases. Use with caution in production.
    /// </summary>
    public async Task<List<string>> GetKeysAsync(string pattern = "*", string? configName = null)
    {
        try
        {
            var server = _context.GetServer(configName);
            var db = _context.GetDatabase(configName);

            // Use SCAN instead of KEYS for better performance
            var keys = server.Keys(db.Database, pattern, pageSize: 250);

            return await Task.Run(() => keys.Select(k => k.ToString()).ToList());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting keys with pattern {Pattern}", pattern);
            throw;
        }
    }

    /// <summary>
    /// Flush all keys in current database
    ///  Warning: This deletes ALL data in the database!
    /// </summary>
    public async Task FlushDatabaseAsync(string? configName = null)
    {
        try
        {
            var server = _context.GetServer(configName);
            var db = _context.GetDatabase(configName);

            _logger.LogWarning(
                "Flushing Redis database {Database}. ALL data will be deleted!",
                db.Database);

            await server.FlushDatabaseAsync(db.Database);

            _logger.LogInformation("Redis database {Database} flushed successfully", db.Database);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error flushing database");
            throw;
        }
    }

    /// <summary>
    /// Ping Redis server to check connectivity
    /// Example: PING
    /// </summary>
    public async Task<TimeSpan> PingAsync(string? configName = null)
    {
        try
        {
            var db = _context.GetDatabase(configName);
            var latency = await db.PingAsync();

            _logger.LogDebug("Redis ping: {Latency}ms", latency.TotalMilliseconds);

            return latency;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error pinging Redis");
            throw;
        }
    }

    /// <summary>
    /// Get Redis server info
    /// </summary>
    public async Task<Dictionary<string, string>> GetServerInfoAsync(string? configName = null)
    {
        try
        {
            var server = _context.GetServer(configName);
            var info = await server.InfoAsync();

            var result = new Dictionary<string, string>();

            foreach (var section in info)
            {
                foreach (var item in section)
                {
                    result[$"{section.Key}:{item.Key}"] = item.Value;
                }
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting server info");
            throw;
        }
    }

    /// <summary>
    /// Get database size (number of keys)
    /// Example: DBSIZE
    /// </summary>
    public async Task<long> GetDatabaseSizeAsync(string? configName = null)
    {
        try
        {
            var server = _context.GetServer(configName);
            var db = _context.GetDatabase(configName);

            return await server.DatabaseSizeAsync(db.Database);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting database size");
            throw;
        }
    }
}
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Kootam.Framework.Caching.Redis.Repository;

/// <summary>
/// Redis Hash operations
/// </summary>
public partial class RedisRepository
{
    public async Task<bool> HashSetAsync(string key, string field, string value, string? configName = null)
    {
        try
        {
            var db = _context.GetDatabase(configName);
            return await db.HashSetAsync(key, field, value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting hash field {Field} in key {Key}", field, key);
            throw;
        }
    }

    public async Task<string?> HashGetAsync(string key, string field, string? configName = null)
    {
        try
        {
            var db = _context.GetDatabase(configName);
            var value = await db.HashGetAsync(key, field);
            return value.HasValue ? value.ToString() : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting hash field {Field} from key {Key}", field, key);
            throw;
        }
    }

    public async Task<Dictionary<string, string>> HashGetAllAsync(string key, string? configName = null)
    {
        try
        {
            var db = _context.GetDatabase(configName);
            var entries = await db.HashGetAllAsync(key);

            return entries.ToDictionary(
                e => e.Name.ToString(),
                e => e.Value.ToString()
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all hash fields from key {Key}", key);
            throw;
        }
    }

    public async Task<bool> HashDeleteAsync(string key, string field, string? configName = null)
    {
        try
        {
            var db = _context.GetDatabase(configName);
            return await db.HashDeleteAsync(key, field);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting hash field {Field} from key {Key}", field, key);
            throw;
        }
    }

    public async Task<bool> HashExistsAsync(string key, string field, string? configName = null)
    {
        try
        {
            var db = _context.GetDatabase(configName);
            return await db.HashExistsAsync(key, field);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking hash field {Field} in key {Key}", field, key);
            throw;
        }
    }
}
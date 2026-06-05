using System.Text.Json;
using Kootam.Caching.Redis.Context;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Kootam.Caching.Redis.Repository;

/// <summary>
/// Redis repository implementation using StackExchange.Redis
/// Thread-safe and should be registered as Singleton
/// 
/// This partial class is split across multiple files:
/// - RedisRepository.cs: Constructor, fields, string & key operations
/// - RedisRepository.Hash.cs: Hash operations (HSET, HGET, etc.)
/// - RedisRepository.Collections.cs: List, Set, Sorted Set operations
/// - RedisRepository.Advanced.cs: Pub/Sub and database operations
/// </summary>
public partial class RedisRepository : IRedisRepository
{
    private readonly IRedisContext _context;
    private readonly ILogger<RedisRepository> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public RedisRepository(
        IRedisContext context,
        ILogger<RedisRepository> logger)
    {
        _context = context;
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            WriteIndented = false,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };
    }

    // ==================== String Operations ====================

    public async Task<string?> GetAsync(string key, string? configName = null)
    {
        try
        {
            var db = _context.GetDatabase(configName);
            var value = await db.StringGetAsync(key);
            return value.HasValue ? value.ToString() : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting key {Key} from Redis", key);
            throw;
        }
    }

    public async Task<T?> GetAsync<T>(string key, string? configName = null) where T : class
    {
        var json = await GetAsync(key, configName);

        if (string.IsNullOrWhiteSpace(json))
            return null;

        try
        {
            return JsonSerializer.Deserialize<T>(json, _jsonOptions);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Error deserializing value for key {Key}", key);
            return null;
        }
    }

    public async Task<bool> SetAsync(string key, string value, TimeSpan? expiry = null, string? configName = null)
    {
        try
        {
            var db = _context.GetDatabase(configName);
            return await db.StringSetAsync(key, value, (Expiration)expiry);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting key {Key} in Redis", key);
            throw;
        }
    }

    public async Task<bool> SetAsync<T>(string key, T value, TimeSpan? expiry = null, string? configName = null) where T : class
    {
        try
        {
            var json = JsonSerializer.Serialize(value, _jsonOptions);
            return await SetAsync(key, json, expiry, configName);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Error serializing value for key {Key}", key);
            throw;
        }
    }

    public async Task<bool> SetIfNotExistsAsync(string key, string value, TimeSpan? expiry = null, string? configName = null)
    {
        try
        {
            var db = _context.GetDatabase(configName);
            return await db.StringSetAsync(key, value, expiry, When.NotExists);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting key {Key} if not exists in Redis", key);
            throw;
        }
    }

    public async Task<bool> SetManyAsync(Dictionary<string, string> keyValues, string? configName = null)
    {
        try
        {
            var db = _context.GetDatabase(configName);
            var pairs = keyValues.Select(kvp =>
                new KeyValuePair<RedisKey, RedisValue>(kvp.Key, kvp.Value)).ToArray();
            return await db.StringSetAsync(pairs);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting multiple keys in Redis");
            throw;
        }
    }

    public async Task<Dictionary<string, string?>> GetManyAsync(IEnumerable<string> keys, string? configName = null)
    {
        try
        {
            var db = _context.GetDatabase(configName);
            var redisKeys = keys.Select(k => (RedisKey)k).ToArray();
            var values = await db.StringGetAsync(redisKeys);

            var result = new Dictionary<string, string?>();
            for (int i = 0; i < redisKeys.Length; i++)
            {
                result[redisKeys[i].ToString()] = values[i].HasValue ? values[i].ToString() : null;
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting multiple keys from Redis");
            throw;
        }
    }

    // ==================== Key Operations ====================

    public async Task<bool> RemoveAsync(string key, string? configName = null)
    {
        try
        {
            var db = _context.GetDatabase(configName);
            return await db.KeyDeleteAsync(key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing key {Key} from Redis", key);
            throw;
        }
    }

    public async Task<long> RemoveManyAsync(IEnumerable<string> keys, string? configName = null)
    {
        try
        {
            var db = _context.GetDatabase(configName);
            var redisKeys = keys.Select(k => (RedisKey)k).ToArray();
            return await db.KeyDeleteAsync(redisKeys);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing multiple keys from Redis");
            throw;
        }
    }

    public async Task<bool> ExistsAsync(string key, string? configName = null)
    {
        try
        {
            var db = _context.GetDatabase(configName);
            return await db.KeyExistsAsync(key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if key {Key} exists in Redis", key);
            throw;
        }
    }

    public async Task<TimeSpan?> GetTimeToLiveAsync(string key, string? configName = null)
    {
        try
        {
            var db = _context.GetDatabase(configName);
            return await db.KeyTimeToLiveAsync(key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting TTL for key {Key} from Redis", key);
            throw;
        }
    }

    public async Task<bool> SetExpirationAsync(string key, TimeSpan expiry, string? configName = null)
    {
        try
        {
            var db = _context.GetDatabase(configName);
            return await db.KeyExpireAsync(key, expiry);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting expiration for key {Key} in Redis", key);
            throw;
        }
    }

    public async Task<bool> RemoveExpirationAsync(string key, string? configName = null)
    {
        try
        {
            var db = _context.GetDatabase(configName);
            return await db.KeyPersistAsync(key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing expiration for key {Key} in Redis", key);
            throw;
        }
    }

    public async Task<bool> RenameAsync(string oldKey, string newKey, string? configName = null)
    {
        try
        {
            var db = _context.GetDatabase(configName);
            return await db.KeyRenameAsync(oldKey, newKey);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error renaming key from {OldKey} to {NewKey} in Redis", oldKey, newKey);
            throw;
        }
    }

    // ==================== Numeric Operations ====================

    public async Task<long> IncrementAsync(string key, long value = 1, string? configName = null)
    {
        try
        {
            var db = _context.GetDatabase(configName);
            return await db.StringIncrementAsync(key, value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error incrementing key {Key} in Redis", key);
            throw;
        }
    }

    public async Task<long> DecrementAsync(string key, long value = 1, string? configName = null)
    {
        try
        {
            var db = _context.GetDatabase(configName);
            return await db.StringDecrementAsync(key, value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error decrementing key {Key} in Redis", key);
            throw;
        }
    }

    public async Task<double> IncrementAsync(string key, double value, string? configName = null)
    {
        try
        {
            var db = _context.GetDatabase(configName);
            return await db.StringIncrementAsync(key, value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error incrementing key {Key} (float) in Redis", key);
            throw;
        }
    }
}
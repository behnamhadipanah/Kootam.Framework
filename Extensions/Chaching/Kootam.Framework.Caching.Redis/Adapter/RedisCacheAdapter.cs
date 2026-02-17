
using Kootam.Framework.Caching.Abstractions;
using Kootam.Framework.Caching.Redis.Repository;
using Microsoft.Extensions.Logging;


namespace Kootam.Framework.Caching.Redis.Adapter;

/// <summary>
/// High-level cache adapter implementation
/// Provides simplified API for common caching patterns
/// </summary>
public class RedisCacheAdapter : ICacheAdapter
{
    private readonly IRedisRepository _repository;
    private readonly ILogger<RedisCacheAdapter> _logger;
    private readonly string? _configName;

    public RedisCacheAdapter(
        IRedisRepository repository,
        ILogger<RedisCacheAdapter> logger,
        string? configName = null)
    {
        _repository = repository;
        _logger = logger;
        _configName = configName;
    }

    public async Task<T?> GetAsync<T>(string key) where T : class
    {
        try
        {
            _logger.LogDebug("Getting cache key: {Key}", key);
            var result = await _repository.GetAsync<T>(key, _configName);

            if (result != null)
                _logger.LogDebug("Cache hit for key: {Key}", key);
            else
                _logger.LogDebug("Cache miss for key: {Key}", key);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting cache key: {Key}", key);
            return null;
        }
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null) where T : class
    {
        try
        {
            _logger.LogDebug("Setting cache key: {Key} (Expiry: {Expiry})", key, expiry);
            await _repository.SetAsync(key, value, expiry, _configName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting cache key: {Key}", key);
            throw;
        }
    }

    public async Task<bool> RemoveAsync(string key)
    {
        try
        {
            _logger.LogDebug("Removing cache key: {Key}", key);
            return await _repository.RemoveAsync(key, _configName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing cache key: {Key}", key);
            return false;
        }
    }

    public async Task<bool> ExistsAsync(string key)
    {
        try
        {
            return await _repository.ExistsAsync(key, _configName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking cache key existence: {Key}", key);
            return false;
        }
    }

    public async Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiry = null) where T : class
    {
        // Try to get from cache
        var cached = await GetAsync<T>(key);
        if (cached != null)
        {
            _logger.LogDebug("Cache hit for key: {Key}", key);
            return cached;
        }

        // Cache miss - execute factory
        _logger.LogDebug("Cache miss for key: {Key}, executing factory", key);

        try
        {
            var value = await factory();

            // Store in cache
            if (value != null)
            {
                await SetAsync(key, value, expiry);
                _logger.LogDebug("Cached value for key: {Key}", key);
            }

            return value;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing factory for key: {Key}", key);
            throw;
        }
    }

    public async Task<T> GetOrSetAsync<T>(string key, Func<T> factory, TimeSpan? expiry = null) where T : class
    {
        return await GetOrSetAsync(key, () => Task.FromResult(factory()), expiry);
    }

    public async Task<long> RemoveByPatternAsync(string pattern)
    {
        try
        {
            _logger.LogDebug("Removing cache keys by pattern: {Pattern}", pattern);

            var keys = await _repository.GetKeysAsync(pattern, _configName);

            if (!keys.Any())
            {
                _logger.LogDebug("No keys found matching pattern: {Pattern}", pattern);
                return 0;
            }

            var count = await _repository.RemoveManyAsync(keys, _configName);
            _logger.LogInformation("Removed {Count} cache keys matching pattern: {Pattern}", count, pattern);

            return count;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing cache keys by pattern: {Pattern}", pattern);
            return 0;
        }
    }

    public async Task<T> GetOrRefreshAsync<T>(string key, Func<Task<T>> factory, TimeSpan expiry) where T : class
    {
        // Try to get from cache
        var cached = await GetAsync<T>(key);

        if (cached != null)
        {
            // Refresh TTL
            await _repository.SetExpirationAsync(key, expiry, _configName);
            _logger.LogDebug("Refreshed TTL for cache key: {Key}", key);
            return cached;
        }

        // Cache miss - execute factory
        _logger.LogDebug("Cache miss for key: {Key}, executing factory", key);
        var value = await factory();

        if (value != null)
        {
            await SetAsync(key, value, expiry);
        }

        return value;
    }

    public async Task SetManyAsync<T>(Dictionary<string, T> items, TimeSpan? expiry = null) where T : class
    {
        try
        {
            _logger.LogDebug("Setting {Count} cache items", items.Count);

            foreach (var item in items)
            {
                await SetAsync(item.Key, item.Value, expiry);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting multiple cache items");
            throw;
        }
    }

    public async Task<Dictionary<string, T?>> GetManyAsync<T>(IEnumerable<string> keys) where T : class
    {
        try
        {
            var keyList = keys.ToList();
            _logger.LogDebug("Getting {Count} cache items", keyList.Count);

            var result = new Dictionary<string, T?>();

            foreach (var key in keyList)
            {
                result[key] = await GetAsync<T>(key);
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting multiple cache items");
            throw;
        }
    }

    public async Task<TimeSpan?> GetTimeToLiveAsync(string key)
    {
        try
        {
            return await _repository.GetTimeToLiveAsync(key, _configName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting TTL for key: {Key}", key);
            return null;
        }
    }

    public async Task<bool> RefreshAsync(string key, TimeSpan expiry)
    {
        try
        {
            var exists = await ExistsAsync(key);

            if (!exists)
            {
                _logger.LogDebug("Key {Key} does not exist, cannot refresh", key);
                return false;
            }

            return await _repository.SetExpirationAsync(key, expiry, _configName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error refreshing cache key: {Key}", key);
            return false;
        }
    }
}
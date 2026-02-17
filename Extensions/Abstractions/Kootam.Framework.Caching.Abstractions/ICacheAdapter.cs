namespace Kootam.Framework.Caching.Abstractions;

/// <summary>
/// High-level cache adapter interface for common caching patterns
/// </summary>
public interface ICacheAdapter
{
    /// <summary>
    /// Get cached value
    /// </summary>
    Task<T?> GetAsync<T>(string key) where T : class;

    /// <summary>
    /// Set cached value
    /// </summary>
    Task SetAsync<T>(string key, T value, TimeSpan? expiry = null) where T : class;

    /// <summary>
    /// Remove cached value
    /// </summary>
    Task<bool> RemoveAsync(string key);

    /// <summary>
    /// Check if key exists in cache
    /// </summary>
    Task<bool> ExistsAsync(string key);

    /// <summary>
    /// Get value from cache, or execute factory and cache the result
    /// </summary>
    Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiry = null) where T : class;

    /// <summary>
    /// Get value from cache, or execute factory and cache the result (synchronous factory)
    /// </summary>
    Task<T> GetOrSetAsync<T>(string key, Func<T> factory, TimeSpan? expiry = null) where T : class;

    /// <summary>
    /// Remove multiple keys by pattern (e.g., "user:*")
    /// </summary>
    Task<long> RemoveByPatternAsync(string pattern);

    /// <summary>
    /// Get or refresh cached value (reset TTL if exists, otherwise create)
    /// </summary>
    Task<T> GetOrRefreshAsync<T>(string key, Func<Task<T>> factory, TimeSpan expiry) where T : class;

    /// <summary>
    /// Set multiple values at once
    /// </summary>
    Task SetManyAsync<T>(Dictionary<string, T> items, TimeSpan? expiry = null) where T : class;

    /// <summary>
    /// Get multiple values at once
    /// </summary>
    Task<Dictionary<string, T?>> GetManyAsync<T>(IEnumerable<string> keys) where T : class;

    /// <summary>
    /// Get time remaining until expiration
    /// </summary>
    Task<TimeSpan?> GetTimeToLiveAsync(string key);

    /// <summary>
    /// Refresh TTL for existing key
    /// </summary>
    Task<bool> RefreshAsync(string key, TimeSpan expiry);
}
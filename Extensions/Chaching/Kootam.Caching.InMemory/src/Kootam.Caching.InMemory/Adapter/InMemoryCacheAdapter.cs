using System.Collections.Concurrent;
using Kootam.Caching.Abstractions;

namespace Kootam.Caching.InMemory.Adapter;

public class InMemoryCacheAdapter : ICacheStore
{
    private readonly ConcurrentDictionary<string, object> _store = new();

    public Task<T?> GetAsync<T>(string key) where T : class
    {
        _store.TryGetValue(key, out var value);
        return Task.FromResult(value as T);
    }

    public Task<bool> ExistsAsync(string key)
    {
        var exists = _store.ContainsKey(key);
        return Task.FromResult(exists);
    }

    public Task SetAsync<T>(string key, T value, TimeSpan? expiry = null) where T : class
    {
        _store[key] = value;
        return Task.CompletedTask;
    }

    public Task<bool> RemoveAsync(string key)
    {
        var removed = _store.TryRemove(key, out _);
        return Task.FromResult(removed);    }
}
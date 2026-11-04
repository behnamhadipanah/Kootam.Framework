using System.Collections.Concurrent;
using Kootam.Caching.Abstractions;

namespace Kootam.Caching.InMemory.Adapter;

public class InMemoryCacheAdapter : ICacheStore
{
    private readonly ConcurrentDictionary<string, Entry> store = new();
    private readonly TimeProvider clock;

    public InMemoryCacheAdapter(TimeProvider? timeProvider = null)
    {
        clock = timeProvider ?? TimeProvider.System;
    }

    public Task<T?> GetAsync<T>(string key) where T : class
        => Task.FromResult(Read(key) as T);

    public Task<bool> ExistsAsync(string key)
        => Task.FromResult(Read(key) is not null);

    public Task SetAsync<T>(string key, T value, TimeSpan? expiry = null) where T : class
    {
        DateTimeOffset? expiresAt = expiry is null ? null : clock.GetUtcNow().Add(expiry.Value);
        store[key] = new Entry(value, expiresAt);
        return Task.CompletedTask;
    }

    public Task<bool> RemoveAsync(string key)
        => Task.FromResult(store.TryRemove(key, out _));

    private object? Read(string key)
    {
        if (!store.TryGetValue(key, out var entry))
            return null;

        if (entry.ExpiresAt is { } expiresAt && expiresAt <= clock.GetUtcNow())
        {
            store.TryRemove(new KeyValuePair<string, Entry>(key, entry));
            return null;
        }

        return entry.Value;
    }

    private sealed record Entry(object Value, DateTimeOffset? ExpiresAt);
}

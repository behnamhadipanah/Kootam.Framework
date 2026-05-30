namespace Kootam.Extensions.Caching.Abstractions;

public interface ICacheGetOrSet
{
    Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiry = null) where T : class;
    Task<T> GetOrSetAsync<T>(string key, Func<T> factory, TimeSpan? expiry = null) where T : class;
}
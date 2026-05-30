namespace Kootam.Extensions.Caching.Abstractions;

public interface ICacheWriter
{
    Task SetAsync<T>(string key, T value, TimeSpan? expiry = null) where T : class;
    Task<bool> RemoveAsync(string key);
}
namespace Kootam.Extensions.Caching.Abstractions;

public interface ICacheReader
{
    Task<T?> GetAsync<T>(string key) where T : class;
    Task<bool> ExistsAsync(string key);
}
namespace Kootam.Extensions.Caching.Abstractions;

public interface ICacheGetOrRefresh
{
    Task<T> GetOrRefreshAsync<T>(string key, Func<Task<T>> factory, TimeSpan expiry) where T : class;
}
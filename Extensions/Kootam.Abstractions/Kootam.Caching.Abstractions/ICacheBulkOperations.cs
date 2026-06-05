namespace Kootam.Caching.Abstractions;

public interface ICacheBulkOperations
{
    Task SetManyAsync<T>(Dictionary<string, T> items, TimeSpan? expiry = null) where T : class;
    Task<Dictionary<string, T?>> GetManyAsync<T>(IEnumerable<string> keys) where T : class;
}
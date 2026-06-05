namespace Kootam.Caching.Redis.Repository;

/// <summary>
/// Repository for Redis operations with support for multiple databases
/// </summary>
public interface IRedisRepository
{
    Task<string?> GetAsync(string key, string? configName = null);

    Task<T?> GetAsync<T>(string key, string? configName = null) where T : class;

    Task<bool> SetAsync(string key, string value, TimeSpan? expiry = null, string? configName = null);

    Task<bool> SetAsync<T>(string key, T value, TimeSpan? expiry = null, string? configName = null) where T : class;

    Task<bool> SetIfNotExistsAsync(string key, string value, TimeSpan? expiry = null, string? configName = null);

    Task<bool> SetManyAsync(Dictionary<string, string> keyValues, string? configName = null);

    Task<Dictionary<string, string?>> GetManyAsync(IEnumerable<string> keys, string? configName = null);

    Task<bool> RemoveAsync(string key, string? configName = null);

    Task<long> RemoveManyAsync(IEnumerable<string> keys, string? configName = null);

    Task<bool> ExistsAsync(string key, string? configName = null);

    Task<TimeSpan?> GetTimeToLiveAsync(string key, string? configName = null);
    Task<bool> SetExpirationAsync(string key, TimeSpan expiry, string? configName = null);

    Task<bool> RemoveExpirationAsync(string key, string? configName = null);
    Task<bool> RenameAsync(string oldKey, string newKey, string? configName = null);

    Task<long> IncrementAsync(string key, long value = 1, string? configName = null);

    Task<long> DecrementAsync(string key, long value = 1, string? configName = null);
    Task<double> IncrementAsync(string key, double value, string? configName = null);
    Task<bool> HashSetAsync(string key, string field, string value, string? configName = null);
    Task<string?> HashGetAsync(string key, string field, string? configName = null);
    Task<Dictionary<string, string>> HashGetAllAsync(string key, string? configName = null);
    Task<bool> HashDeleteAsync(string key, string field, string? configName = null);
    Task<bool> HashExistsAsync(string key, string field, string? configName = null);
    Task<long> ListPushAsync(string key, string value, string? configName = null);
    Task<string?> ListPopAsync(string key, string? configName = null);
    Task<List<string>> ListRangeAsync(string key, long start = 0, long stop = -1, string? configName = null);
    Task<long> ListLengthAsync(string key, string? configName = null);
    Task<bool> SetAddAsync(string key, string value, string? configName = null);
    Task<bool> SetRemoveAsync(string key, string value, string? configName = null);
    Task<bool> SetContainsAsync(string key, string value, string? configName = null);
    Task<List<string>> SetMembersAsync(string key, string? configName = null);
    Task<long> SetLengthAsync(string key, string? configName = null);
    Task<bool> SortedSetAddAsync(string key, string member, double score, string? configName = null);
    Task<bool> SortedSetRemoveAsync(string key, string member, string? configName = null);
    Task<List<string>> SortedSetRangeAsync(string key, long start = 0, long stop = -1, string? configName = null);
    Task<double?> SortedSetScoreAsync(string key, string member, string? configName = null);
    Task<long> PublishAsync(string channel, string message, string? configName = null);
    Task<List<string>> GetKeysAsync(string pattern = "*", string? configName = null);
    Task FlushDatabaseAsync(string? configName = null);
    Task<TimeSpan> PingAsync(string? configName = null);
}
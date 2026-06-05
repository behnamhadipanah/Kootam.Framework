namespace Kootam.Caching.Abstractions;

/// <summary>
/// TTL / Expiration Operation
/// </summary>
public interface ICacheExpiration
{
    
    Task<TimeSpan?> GetTimeToLiveAsync(string key);
    Task<bool> RefreshAsync(string key, TimeSpan expiry);
}
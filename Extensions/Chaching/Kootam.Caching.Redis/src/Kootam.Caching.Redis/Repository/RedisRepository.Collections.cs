using Microsoft.Extensions.Logging;

namespace Kootam.Caching.Redis.Repository;

/// <summary>
/// Redis List, Set, and Sorted Set operations (Collections)
/// </summary>
public partial class RedisRepository
{

    public async Task<long> ListPushAsync(string key, string value, string? configName = null)
    {
        try
        {
            var db = _context.GetDatabase(configName);
            return await db.ListRightPushAsync(key, value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error pushing to list {Key}", key);
            throw;
        }
    }

    public async Task<string?> ListPopAsync(string key, string? configName = null)
    {
        try
        {
            var db = _context.GetDatabase(configName);
            var value = await db.ListRightPopAsync(key);
            return value.HasValue ? value.ToString() : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error popping from list {Key}", key);
            throw;
        }
    }

    public async Task<List<string>> ListRangeAsync(string key, long start = 0, long stop = -1, string? configName = null)
    {
        try
        {
            var db = _context.GetDatabase(configName);
            var values = await db.ListRangeAsync(key, start, stop);
            return values.Select(v => v.ToString()).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting list range from {Key}", key);
            throw;
        }
    }

    public async Task<long> ListLengthAsync(string key, string? configName = null)
    {
        try
        {
            var db = _context.GetDatabase(configName);
            return await db.ListLengthAsync(key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting list length for {Key}", key);
            throw;
        }
    }

    // ==================== Set Operations ====================

    public async Task<bool> SetAddAsync(string key, string value, string? configName = null)
    {
        try
        {
            var db = _context.GetDatabase(configName);
            return await db.SetAddAsync(key, value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding to set {Key}", key);
            throw;
        }
    }

    public async Task<bool> SetRemoveAsync(string key, string value, string? configName = null)
    {
        try
        {
            var db = _context.GetDatabase(configName);
            return await db.SetRemoveAsync(key, value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing from set {Key}", key);
            throw;
        }
    }

    public async Task<bool> SetContainsAsync(string key, string value, string? configName = null)
    {
        try
        {
            var db = _context.GetDatabase(configName);
            return await db.SetContainsAsync(key, value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking set membership in {Key}", key);
            throw;
        }
    }

    public async Task<List<string>> SetMembersAsync(string key, string? configName = null)
    {
        try
        {
            var db = _context.GetDatabase(configName);
            var members = await db.SetMembersAsync(key);
            return members.Select(m => m.ToString()).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting set members from {Key}", key);
            throw;
        }
    }

    public async Task<long> SetLengthAsync(string key, string? configName = null)
    {
        try
        {
            var db = _context.GetDatabase(configName);
            return await db.SetLengthAsync(key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting set length for {Key}", key);
            throw;
        }
    }

    // ==================== Sorted Set Operations ====================

    public async Task<bool> SortedSetAddAsync(string key, string member, double score, string? configName = null)
    {
        try
        {
            var db = _context.GetDatabase(configName);
            return await db.SortedSetAddAsync(key, member, score);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding to sorted set {Key}", key);
            throw;
        }
    }

    public async Task<bool> SortedSetRemoveAsync(string key, string member, string? configName = null)
    {
        try
        {
            var db = _context.GetDatabase(configName);
            return await db.SortedSetRemoveAsync(key, member);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing from sorted set {Key}", key);
            throw;
        }
    }

    public async Task<List<string>> SortedSetRangeAsync(string key, long start = 0, long stop = -1, string? configName = null)
    {
        try
        {
            var db = _context.GetDatabase(configName);
            var values = await db.SortedSetRangeByRankAsync(key, start, stop);
            return values.Select(v => v.ToString()).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting sorted set range from {Key}", key);
            throw;
        }
    }

    public async Task<double?> SortedSetScoreAsync(string key, string member, string? configName = null)
    {
        try
        {
            var db = _context.GetDatabase(configName);
            return await db.SortedSetScoreAsync(key, member);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting score from sorted set {Key}", key);
            throw;
        }
    }
}
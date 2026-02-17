using Kootam.Framework.Authentication.Jwt.Abstractions;
using Kootam.Framework.Authentication.Jwt.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Kootam.Framework.Authentication.Jwt.Storages;

public class InMemoryTokenStore(IMemoryCache cache,ILogger<InMemoryTokenStore> logger) : ITokenStore
{
    private readonly TimeSpan _defaultExpiration = TimeSpan.FromDays(7);

    public Task<List<DeviceTokenInfo>> GetUserTokensAsync(string userKey)
    {
        try
        {
            if (cache.TryGetValue(GetCacheKey(userKey), out List<DeviceTokenInfo> tokens))
            {
                return Task.FromResult(tokens);
            }
            return Task.FromResult(new List<DeviceTokenInfo>());
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving tokens from memory for user {UserKey}", userKey);
            return Task.FromResult(new List<DeviceTokenInfo>());
        }
    }

    public Task SaveUserTokensAsync(string userKey, List<DeviceTokenInfo> tokens)
    {
        try
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(_defaultExpiration)
                .SetPriority(CacheItemPriority.Normal);

            cache.Set(GetCacheKey(userKey), tokens, cacheEntryOptions);
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error saving tokens to memory for user {UserKey}", userKey);
            throw;
        }
    }

    public Task RemoveUserTokensAsync(string userKey)
    {
        try
        {
            cache.Remove(GetCacheKey(userKey));
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error removing tokens from memory for user {UserKey}", userKey);
            return Task.CompletedTask;
        }
    }

    public async Task<bool> IsTokenValidAsync(string userKey, string loginValidationKey)
    {
        var tokens = await GetUserTokensAsync(userKey);
        return tokens.Any();
    }

    private string GetCacheKey(string userKey) => $"UserTokens_{userKey}";
}

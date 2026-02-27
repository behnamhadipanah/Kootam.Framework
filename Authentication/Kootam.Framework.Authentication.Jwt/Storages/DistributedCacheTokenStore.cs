using Kootam.Framework.Authentication.Jwt.Abstractions;
using Kootam.Framework.Authentication.Jwt.Models;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Kootam.Framework.Authentication.Jwt.Storages;

public class DistributedCacheTokenStore(IDistributedCache cache, ILogger<DistributedCacheTokenStore> logger) : ITokenStore
{
    private readonly TimeSpan _defaultExpiration = TimeSpan.FromDays(7);


    public async Task<List<DeviceTokenInfo>> GetUserTokensAsync(string userKey)
    {
        try
        {
            var data = await cache.GetStringAsync(GetCacheKey(userKey));
            if (string.IsNullOrEmpty(data))
                return new List<DeviceTokenInfo>();

            return JsonConvert.DeserializeObject<List<DeviceTokenInfo>>(data);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving tokens from distributed cache for user {UserKey}", userKey);
            return new List<DeviceTokenInfo>();
        }
    }

    public async Task SaveUserTokensAsync(string userKey, List<DeviceTokenInfo> tokens)
    {
        try
        {
            var options = new DistributedCacheEntryOptions
            {
                SlidingExpiration = _defaultExpiration
            };

            var data = JsonConvert.SerializeObject(tokens);
            await cache.SetStringAsync(GetCacheKey(userKey), data, options);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error saving tokens to distributed cache for user {UserKey}", userKey);
            throw;
        }
    }

    public async Task RemoveUserTokensAsync(string userKey)
    {
        try
        {
            await cache.RemoveAsync(GetCacheKey(userKey));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error removing tokens from distributed cache for user {UserKey}", userKey);
        }
    }

    public async Task<bool> IsTokenValidAsync(string userKey, string loginValidationKey)
    {
        var tokens = await GetUserTokensAsync(userKey);
        return tokens.Any();
    }

    private string GetCacheKey(string userKey) => $"UserTokens_{userKey}";
}
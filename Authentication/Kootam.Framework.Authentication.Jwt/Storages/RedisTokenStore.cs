using Kootam.Framework.Authentication.Jwt.Abstractions;
using Kootam.Framework.Authentication.Jwt.Models;
using Kootam.Framework.Caching.Redis.Repository;
using Kootam.Framework.Utilities.Common.Extensions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Kootam.Framework.Authentication.Jwt.Storages;

public class RedisTokenStore(IRedisRepository redisRepository, ILogger<RedisTokenStore> logger) : ITokenStore
{


    public async Task<List<DeviceTokenInfo>> GetUserTokensAsync(string userKey)
    {
        try
        {
            var redisData = await redisRepository.GetAsync(userKey);

            if (redisData.IsNullOrEmpty())
                return new List<DeviceTokenInfo>();

            return JsonConvert.DeserializeObject<List<DeviceTokenInfo>>(redisData);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving tokens for user {UserKey}", userKey);
            return new List<DeviceTokenInfo>();
        }
    }

    public async Task SaveUserTokensAsync(string userKey, List<DeviceTokenInfo> tokens)
    {
        try
        {
            await redisRepository.SetAsync(userKey, JsonConvert.SerializeObject(tokens));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error saving tokens for user {UserKey}", userKey);
            throw;
        }
    }

    public async Task RemoveUserTokensAsync(string userKey)
    {
        try
        {
            await redisRepository.RemoveAsync(userKey);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error removing tokens for user {UserKey}", userKey);
        }
    }

    public async Task<bool> IsTokenValidAsync(string userKey, string loginValidationKey)
    {
        var tokens = await GetUserTokensAsync(userKey);
        return tokens.Any(); // Redis presence means valid
    }
}

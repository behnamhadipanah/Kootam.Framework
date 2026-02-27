using Kootam.Framework.Authentication.Jwt.Models;

namespace Kootam.Framework.Authentication.Jwt.Abstractions;

public interface ITokenStore
{
    Task<List<DeviceTokenInfo>> GetUserTokensAsync(string userKey);
    Task SaveUserTokensAsync(string userKey, List<DeviceTokenInfo> tokens);
    Task RemoveUserTokensAsync(string userKey);
    Task<bool> IsTokenValidAsync(string userKey, string loginValidationKey);
}

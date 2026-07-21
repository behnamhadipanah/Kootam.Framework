using Kootam.Authentication.Abstractions.Models;

namespace Kootam.Authentication.Abstractions.Tokens;

public interface ITokenGenerator<TUser,TUserKey>
{
    IssuedAccessToken GenerateAccessToken(TUser user);

    RefreshToken<TUserKey> GenerateRefreshToken(TUserKey userKey,string ipAddress);
}
using Kootam.Authentication.Abstractions.Models;

namespace Kootam.Authentication.Abstractions.Tokens;

public interface ITokenGenerator<TUser>
{
    IssuedAccessToken GenerateAccessToken(TUser user);

    RefreshToken GenerateRefreshToken(string ipAddress);
}
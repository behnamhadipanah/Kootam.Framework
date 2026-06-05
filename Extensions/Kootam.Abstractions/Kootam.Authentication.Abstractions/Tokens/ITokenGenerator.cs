using Kootam.Extensions.Authentication.Abstractions.Models;

namespace Kootam.Extensions.Authentication.Abstractions.Tokens;

public interface ITokenGenerator<TUser>
{
    string GenerateAccessToken(TUser user);

    RefreshToken GenerateRefreshToken(string ipAddress);
}

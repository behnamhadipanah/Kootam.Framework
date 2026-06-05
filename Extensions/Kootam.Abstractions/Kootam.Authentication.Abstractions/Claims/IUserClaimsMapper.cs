using System.Security.Claims;

namespace Kootam.Authentication.Abstractions.Claims;

public interface IUserClaimsMapper<TUser>
{
    List<Claim> MapToClaims(TUser user);

}

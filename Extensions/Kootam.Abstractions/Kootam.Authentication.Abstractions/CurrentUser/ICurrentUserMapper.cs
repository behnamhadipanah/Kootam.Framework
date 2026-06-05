using System.Security.Claims;

namespace Kootam.Authentication.Abstractions.CurrentUser;

public interface ICurrentUserMapper<TUser>
{
    TUser Map(ClaimsPrincipal principal);

}

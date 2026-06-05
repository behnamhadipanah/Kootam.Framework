using System.Security.Claims;

namespace Kootam.Extensions.Authentication.Abstractions.CurrentUser;

public interface ICurrentUserMapper<TUser>
{
    TUser Map(ClaimsPrincipal principal);

}

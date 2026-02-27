namespace Kootam.Framework.Authentication.Jwt.Abstractions;

public interface IDelegationService
{
    Task<UserClaimsPrincipal> ResolveDelegationAsync(UserClaimsPrincipal principal);
}

using System.Security.Claims;

namespace Kootam.UserManagement.DependencyInjection;

public static class ClaimExtensions
{
    public static string GetClaim(this ClaimsPrincipal  claimsPrincipal,string claimType)
    {
        return claimsPrincipal.Claims.FirstOrDefault((Claim x) => x.Type == claimType)?.Value;
    }
}

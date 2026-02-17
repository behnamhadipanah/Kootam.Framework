using Kootam.Framework.Authentication.Jwt.Models;
using System.Security.Claims;

namespace Kootam.Framework.Authentication.Jwt.Abstractions;

public interface IUserClaimsMapper
{
    UserClaimsPrincipal MapFromClaims(IEnumerable<Claim> claims, string token);
    IEnumerable<Claim> MapToClaims(UserClaimsPrincipal principal);
}
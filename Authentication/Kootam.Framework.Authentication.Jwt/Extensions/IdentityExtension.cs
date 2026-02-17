using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Kootam.Framework.Authentication.Jwt.Extensions;

public static class IdentityExtension
{
    public static string GetUserEmail(this ClaimsPrincipal? claimsPrincipal)
    {
        if (claimsPrincipal == null)
            return String.Empty;

        var result = claimsPrincipal.FindFirst(ClaimTypes.Email);
        return result.Value;


    }
}

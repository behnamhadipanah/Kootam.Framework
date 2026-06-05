using System.Security.Claims;
using Kootam.Authentication.Abstractions.Enums;
using Kootam.Authentication.Abstractions.Handlers;
using Kootam.Authentication.Abstractions.Models;
using Kootam.Authentication.Abstractions.Tokens;
using Microsoft.AspNetCore.Http;

namespace Kootam.Authentication.Jwt.Handlers;

public class JwtAuthHandler : IAuthHandler
{
    private readonly ITokenValidator _validator;

    public string Scheme => "Jwt";

    public JwtAuthHandler(ITokenValidator validator)
    {
        _validator = validator;
    }

    public async Task<AuthResult> AuthenticateAsync(HttpContext context)
    {
        var header = context.Request.Headers["Authorization"].FirstOrDefault();

        if (string.IsNullOrEmpty(header))
            return AuthResult.Fail("Missing Authorization header");

        if (!header.StartsWith("Bearer "))
            return AuthResult.Fail("Invalid scheme");

        var token = header.Substring("Bearer ".Length);

        var result = await _validator.ValidateTokenAsync(token, DeviceType.Desktop);

        if (!result.IsValid)
            return AuthResult.Fail("Invalid token");

        var principal = new ClaimsPrincipal(result.ClaimsIdentity);

        return AuthResult.Success(principal);
    }
}

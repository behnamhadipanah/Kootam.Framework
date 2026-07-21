using System.Security.Claims;
using Kootam.Authentication.Abstractions.Enums;
using Kootam.Authentication.Abstractions.Handlers;
using Kootam.Authentication.Abstractions.Models;
using Kootam.Authentication.Abstractions.Tokens;
using Microsoft.AspNetCore.Http;

namespace Kootam.Authentication.Jwt.Handlers;

public class JwtAuthHandler : IAuthenticateHandler
{
    private readonly ITokenValidator _validator;
    private readonly IAccessTokenReader _reader;

    public string Scheme => "Jwt";

    public JwtAuthHandler(ITokenValidator validator, IAccessTokenReader reader)
    {
        _validator = validator;
        _reader = reader;
    }

    public async Task<AuthenticationResult> AuthenticateAsync(HttpContext context)
    {
        string token=await _reader.ReadAsync(context);
        
        var result = await _validator.ValidateTokenAsync(token, DeviceType.Desktop);

        if (!result.IsValid)
            return AuthenticationResult.Fail("Invalid token");

        var principal = new ClaimsPrincipal(result.ClaimsIdentity);

        return AuthenticationResult.Success(principal);
    }
}

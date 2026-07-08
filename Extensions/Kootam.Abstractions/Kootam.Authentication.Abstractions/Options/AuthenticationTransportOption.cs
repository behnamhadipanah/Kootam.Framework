using Kootam.Authentication.Abstractions.Enums;
using Microsoft.AspNetCore.Http;

namespace Kootam.Authentication.Abstractions.Options;

public class AuthenticationTransportOption
{
    public TransportMode Mode { get; set; } = TransportMode.AuthorizationHeader;

    public string AccessTokenKey { get; set; } = "AccessToken";

    public string RefreshTokenKey { get; set; } = "RefreshToken";

    public string HeaderName { get; set; } = "Authorization";

    public bool HttpOnly { get; set; } = true;

    public bool Secure { get; set; } = true;

    public SameSiteMode SameSite { get; set; } = SameSiteMode.Strict;
}
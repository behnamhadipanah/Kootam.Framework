
using Microsoft.Net.Http.Headers;

namespace Kootam.Authentication.Jwt.Options;

public class JwtOptions
{
    public string Key { get; set; } = "";
    public string Issuer { get; set; } = "";
    public string Audience { get; set; } = "";

    public int AccessTokenMinutes { get; set; } = 15;
    public int RefreshTokenDays { get; set; } = 7;
    public bool MapInboundClaims { get; set; } = false;


}
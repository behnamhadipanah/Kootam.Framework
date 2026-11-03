using Kootam.Authentication.SignIn.Enums;
using Kootam.Authentication.SignIn.Models;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace Kootam.Authentication.SignIn.Services;

public static class OidcIdTokenValidator
{
    private static readonly JsonWebTokenHandler Handler = new();

    public static async Task<OidcTokenDocument?> ValidateAsync(
        string token,
        IEnumerable<SecurityKey> signingKeys,
        IEnumerable<string> audiences,
        OidcIssuerRule issuerRule,
        TimeSpan? clockSkew = null)
    {
        if (string.IsNullOrWhiteSpace(token))
            return null;

        var keys = signingKeys.ToArray();
        var audienceList = audiences.Where(audience => !string.IsNullOrWhiteSpace(audience)).ToArray();
        if (keys.Length == 0 || audienceList.Length == 0)
            return null;

        var result = await Handler.ValidateTokenAsync(token, new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKeys = keys,
            ValidateAudience = true,
            ValidAudiences = audienceList,
            ValidateIssuer = true,
            ValidateLifetime = true,
            ClockSkew = clockSkew ?? TimeSpan.FromMinutes(2),
            IssuerValidator = (issuer, securityToken, _) =>
            {
                if (securityToken is not JsonWebToken jwt || !IssuerAccepted(issuer, jwt, issuerRule))
                    throw new SecurityTokenInvalidIssuerException("Issuer is not accepted.");

                return issuer;
            }
        });

        if (!result.IsValid || result.SecurityToken is not JsonWebToken valid)
            return null;

        var subject = ReadString(valid, JwtRegisteredClaimNames.Sub);
        if (string.IsNullOrWhiteSpace(subject))
            return null;

        var email = ReadString(valid, JwtRegisteredClaimNames.Email);
        return new OidcTokenDocument(
            subject,
            email,
            email is not null && ReadVerified(valid),
            ReadString(valid, JwtRegisteredClaimNames.GivenName),
            ReadString(valid, JwtRegisteredClaimNames.FamilyName));
    }

    private static bool IssuerAccepted(string issuer, JsonWebToken jwt, OidcIssuerRule rule)
        => rule switch
        {
            OidcIssuerRule.Google => issuer is "https://accounts.google.com" or "accounts.google.com",
            OidcIssuerRule.Apple => issuer == "https://appleid.apple.com",
            OidcIssuerRule.Microsoft => MicrosoftIssuerAccepted(issuer, jwt),
            _ => false
        };

    private static bool MicrosoftIssuerAccepted(string issuer, JsonWebToken jwt)
    {
        if (!jwt.TryGetPayloadValue<string>("tid", out var tenantId) || !Guid.TryParse(tenantId, out _))
            return false;

        return string.Equals(issuer, $"https://login.microsoftonline.com/{tenantId}/v2.0", StringComparison.Ordinal);
    }

    private static string? ReadString(JsonWebToken jwt, string claim)
        => jwt.TryGetPayloadValue<string>(claim, out var value) && !string.IsNullOrWhiteSpace(value)
            ? value
            : null;

    private static bool ReadVerified(JsonWebToken jwt)
    {
        if (jwt.TryGetPayloadValue<bool>("email_verified", out var flag))
            return flag;

        return jwt.TryGetPayloadValue<string>("email_verified", out var text)
               && bool.TryParse(text, out var parsed)
               && parsed;
    }
}

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Kootam.Extensions.Authentication.Abstractions.Enums;
using Kootam.Extensions.Authentication.Abstractions.Tokens;
using Kootam.Extensions.Authentication.Jwt.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Kootam.Extensions.Authentication.Jwt.Services;

public class JwtTokenValidator(IOptions<JwtOptions> authConstant) : ITokenValidator
{
    public JwtSecurityToken DecodeToken(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        return handler.ReadJwtToken(token);
    }

    public string GenerateToken(Dictionary<string, string> claims, DateTime? expiration = null)
    {
        var handler = new JwtSecurityTokenHandler();

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(authConstant.Value.Key));

        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256);

        var claimList = claims
            .Select(x => new Claim(x.Key, x.Value));

        var token = new JwtSecurityToken(
            issuer: authConstant.Value.Issuer,
            audience: authConstant.Value.Audience,
            claims: claimList,
            expires: expiration ?? DateTime.UtcNow.AddMinutes(authConstant.Value.AccessTokenMinutes),
            signingCredentials: credentials
        );

        return handler.WriteToken(token);
    }

    public DateTime? GetTokenExpiration(JwtSecurityToken token)
    {
        return token.ValidTo;
    }

    public async Task<TokenValidationResult> ValidateTokenAsync(string token, DeviceType deviceType)
    {
        var handler = new JwtSecurityTokenHandler();

        var parameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = authConstant.Value.Issuer,

            ValidateAudience = true,
            ValidAudience = authConstant.Value.Audience,

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(authConstant.Value.Key)),

            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };

        return await handler.ValidateTokenAsync(token, parameters);
    }
}

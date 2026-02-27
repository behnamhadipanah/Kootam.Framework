using Kootam.Framework.Authentication.Jwt.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Kootam.Framework.Authentication.Jwt.Configurations;


public abstract class JwtConfiguration<T>
{
    private readonly IOptions<AuthConstants> _authConstantsSettings;
    protected JwtConfiguration(IOptions<AuthConstants> authConstantsSettings)
    {
        _authConstantsSettings = authConstantsSettings;
    }
    public abstract List<Claim> CreateClaims(T user);

    public ClaimsIdentity CreateClaimsIdentity(List<Claim> claims)
    {
        return new ClaimsIdentity(
            claims, CookieAuthenticationDefaults.AuthenticationScheme);
    }
    public string GenerateJwtToken(List<Claim> claims)
    {
        var secretBytes = Encoding.UTF8.GetBytes(_authConstantsSettings.Value.Key);
        SymmetricSecurityKey key = new SymmetricSecurityKey(secretBytes);
        var algorithm = SecurityAlgorithms.HmacSha256;

        var signingCredentials = new SigningCredentials(key, algorithm);

        var token = new JwtSecurityToken(
            _authConstantsSettings.Value.Issuer,
            _authConstantsSettings.Value.Audiance,
            claims,
            notBefore: DateTime.Now,
            expires: DateTime.Now.AddMinutes(_authConstantsSettings.Value.ExpireTokenPerMinutes),
            signingCredentials);

        var tokenJson = new JwtSecurityTokenHandler().WriteToken(token);
        return tokenJson;
    }

    public RefreshToken GenerateRefreshToken(string ipAddress)
    {


        byte[] randomBytes = RandomNumberGenerator.GetBytes(64);
        return new RefreshToken
        {
            Token = Convert.ToBase64String(randomBytes),
            Expires = DateTime.UtcNow.AddMinutes(_authConstantsSettings.Value.ExpireTokenPerMinutes),
            Created = DateTime.UtcNow,
            CreatedByIp = ipAddress
        };


    }
}

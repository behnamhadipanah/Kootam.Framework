using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;
using Kootam.Authentication.Abstractions.Claims;
using Kootam.Authentication.Abstractions.Models;
using Kootam.Authentication.Abstractions.Tokens;
using Kootam.Authentication.Jwt.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Kootam.Authentication.Jwt.Services;

public class JwtTokenGenerator<TUser> : ITokenGenerator<TUser>
{
    private readonly JwtOptions _options;
    private readonly IUserClaimsMapper<TUser> _claimsMapper;

    public JwtTokenGenerator(IOptions<JwtOptions> options, IUserClaimsMapper<TUser> claimsMapper)
    {
        _options = options.Value;
        _claimsMapper = claimsMapper;
    }
    public IssuedAccessToken GenerateAccessToken(TUser user)
    {
        var claims = _claimsMapper.MapToClaims(user);
        var expires = DateTime.UtcNow.AddMinutes(_options.AccessTokenMinutes);

        var key = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(_options.Key));

        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
         issuer: _options.Issuer,
         audience: _options.Audience,
         claims: claims,
         notBefore: DateTime.UtcNow,
         expires: expires,
         signingCredentials: credentials);

        return new IssuedAccessToken
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            Expires = expires
        };
    }

    public RefreshToken GenerateRefreshToken(string ipAddress)
    {
        var randomBytes = RandomNumberGenerator.GetBytes(64);

        return new RefreshToken
        {
            Token = Convert.ToBase64String(randomBytes),
            Created = DateTime.UtcNow,
            CreatedByIp = ipAddress,
            Expires = DateTime.UtcNow.AddDays(_options.RefreshTokenDays)
        };
    }
}

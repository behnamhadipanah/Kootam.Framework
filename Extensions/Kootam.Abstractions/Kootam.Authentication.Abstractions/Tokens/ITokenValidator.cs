using System.IdentityModel.Tokens.Jwt;
using Kootam.Authentication.Abstractions.Enums;
using Microsoft.IdentityModel.Tokens;

namespace Kootam.Authentication.Abstractions.Tokens;

public interface ITokenValidator
{
    Task<TokenValidationResult> ValidateTokenAsync(string token, DeviceType deviceType);
    JwtSecurityToken DecodeToken(string token);
    string GenerateToken(Dictionary<string, string> claims, DateTime? expiration = null);
    DateTime? GetTokenExpiration(JwtSecurityToken token);
}

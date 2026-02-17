using Kootam.Framework.Authentication.Jwt.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace Kootam.Framework.Authentication.Jwt.Abstractions;

public interface ITokenValidator
{
    Task<TokenValidationResult> ValidateTokenAsync(string token, string deviceType);
    JwtSecurityToken DecodeToken(string token);
    string GenerateToken(Dictionary<string, string> claims, DateTime? expiration = null);
    DateTime? GetTokenExpiration(JwtSecurityToken token);
}

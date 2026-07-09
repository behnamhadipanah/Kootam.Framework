using System.Security.Claims;
using Kootam.Authentication.Abstractions.Claims;
using Kootam.Authentication.Jwt.Sample.ViewModels;

namespace Kootam.Authentication.Jwt.Sample.Mappers;

public class LoggingUserClaimsMapper : IUserClaimsMapper<LoggingUserViewModel>
{
    public List<Claim> MapToClaims(LoggingUserViewModel user)
    {
        return new List<Claim>
        {
            new Claim(KootamClaimTypes.UserId, user.Id.ToString()),
            new Claim(KootamClaimTypes.Email, user.Email ?? ""),
            new Claim(KootamClaimTypes.Username, user.FirstName ?? ""),
            new Claim(AppClaimTypes.FirstName, user.FirstName ?? ""),
            new Claim(AppClaimTypes.LastName, user.LastName ?? ""),
            new Claim(KootamClaimTypes.Mobile, user.PhoneNumber ?? ""),
            new Claim(KootamClaimTypes.LoginValid, Guid.NewGuid().ToString())
        };
    }
}

public static class AppClaimTypes
{
    public const string FirstName = "first_name";

    public const string LastName = "last_name";
}
using Kootam.Extensions.UserManagement.Abstractions;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Kootam.Extensions.UserManagement.DependencyInjection;


namespace Kootam.Extensions.UserManagement.Services;

public class WebUserInfoService : IUserInfoService
{
    private readonly IHttpContextAccessor _contextAccessor;

    public WebUserInfoService(IHttpContextAccessor contextAccessor)
    {
        if (contextAccessor is null || contextAccessor.HttpContext is null)
            throw new ArgumentNullException(nameof(contextAccessor));

        _contextAccessor = contextAccessor;
    }
    public string? GetClaim(string claimType)
    {
        throw new NotImplementedException();
    }

    public string GetFirstName() => _contextAccessor.HttpContext.User?.GetClaim(ClaimTypes.GivenName);

    public string GetLastName() => _contextAccessor.HttpContext.User?.GetClaim(ClaimTypes.Surname);
    public string GetUserAgent() => _contextAccessor.HttpContext.Request.Headers["UserAgent"];

    public string GetUserIp() => _contextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();


    public string GetUsername() => _contextAccessor.HttpContext.User?.GetClaim(ClaimTypes.Name);


    public bool HasAccess(string access)
    {
        throw new NotImplementedException();
    }

    public bool IsCurrentUser(string userId)
    {
        throw new NotImplementedException();
    }

    public string UserId() => _contextAccessor.HttpContext.User?.GetClaim(ClaimTypes.NameIdentifier);



    public string UserIdOrDefault() => _contextAccessor.HttpContext.User?.GetClaim(ClaimTypes.NameIdentifier);

    public string UserIdOrDefault(string defaultValue) => _contextAccessor.HttpContext.User?.GetClaim(ClaimTypes.NameIdentifier);
}

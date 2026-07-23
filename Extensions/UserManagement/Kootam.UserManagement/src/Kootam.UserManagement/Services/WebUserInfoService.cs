using Kootam.UserManagement.Abstractions;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Kootam.UserManagement.DependencyInjection;


namespace Kootam.UserManagement.Services;

public class WebUserInfoService : IUserInfoService
{
    private readonly IHttpContextAccessor _contextAccessor;

    public WebUserInfoService(IHttpContextAccessor contextAccessor)
    {
        if (contextAccessor is null || contextAccessor.HttpContext is null)
            throw new ArgumentNullException(nameof(contextAccessor));

        _contextAccessor = contextAccessor;
    }

    public string? GetClaim(string claimType) => _contextAccessor.HttpContext.User?.GetClaim(claimType);

    /// <summary>
    /// ClaimTypes is givenName
    /// </summary>
    /// <returns></returns>
    public string GetFirstName() => _contextAccessor.HttpContext.User?.GetClaim(ClaimTypes.GivenName);

    /// <summary>
    /// ClaimTypes is surname
    /// </summary>
    /// <returns></returns>
    public string GetLastName() => _contextAccessor.HttpContext.User?.GetClaim(ClaimTypes.Surname);

    /// <summary>
    /// Get user agent from header
    /// </summary>
    /// <returns></returns>
    public string GetUserAgent() => _contextAccessor.HttpContext.Request.Headers["UserAgent"];

    /// <summary>
    /// Get remote IP address
    /// </summary>
    /// <returns></returns>
    public string GetUserIp() => _contextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();

    /// <summary>
    /// ClaimTypes is name
    /// </summary>
    /// <returns></returns>
    public string GetUsername() => _contextAccessor.HttpContext.User?.GetClaim(ClaimTypes.Name);


    public bool HasAccess(string claimType, string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;

        var user = _contextAccessor.HttpContext?.User;

        if (user?.Identity?.IsAuthenticated != true)
            return false;

        return user.Claims.Any(c =>
            c.Type.Equals(claimType, StringComparison.OrdinalIgnoreCase) &&
            c.Value.Equals(value, StringComparison.OrdinalIgnoreCase));
    }

    public bool IsCurrentUser(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
            return false;

        var currentUserId = UserIdOrDefault();

        return !string.IsNullOrWhiteSpace(currentUserId) &&
               string.Equals(currentUserId, userId, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// ClaimTypes is nameIdentifier
    /// </summary>
    /// <returns></returns>
    public string UserId() => _contextAccessor.HttpContext.User?.GetClaim(ClaimTypes.NameIdentifier);

    /// <summary>
    /// ClaimTypes is nameIdentifier
    /// </summary>
    /// <returns></returns>
    public string UserIdOrDefault() => _contextAccessor.HttpContext.User?.GetClaim(ClaimTypes.NameIdentifier);

    /// <summary>
    /// ClaimTypes is nameIdentifier with default value
    /// </summary>
    /// <returns></returns>
    public string UserIdOrDefault(string defaultValue) =>
        _contextAccessor.HttpContext.User?.GetClaim(ClaimTypes.NameIdentifier);
}
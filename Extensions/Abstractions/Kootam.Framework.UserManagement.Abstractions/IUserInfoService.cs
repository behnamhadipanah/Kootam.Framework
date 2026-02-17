namespace Kootam.Framework.UserManagement.Abstractions;
public interface IUserInfoService
{
    string GetUserAgent();
    string GetUserIp();
    string UserId();
    string GetFirstName();
    string GetLastName();
    string GetUsername();
    string? GetClaim(string claimType);
    bool IsCurrentUser(string userId);
    bool HasAccess(string access);
    string UserIdOrDefault();
    string UserIdOrDefault(string defaultValue);
}

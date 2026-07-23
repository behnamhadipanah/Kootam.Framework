namespace Kootam.UserManagement.Abstractions;

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
    bool HasAccess(string claimType,string value);
    string UserIdOrDefault();
    string UserIdOrDefault(string defaultValue);
}

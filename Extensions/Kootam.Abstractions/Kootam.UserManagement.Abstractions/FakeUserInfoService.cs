namespace Kootam.UserManagement.Abstractions;

public class FakeUserInfoService : IUserInfoService
{
    public string? GetClaim(string claimType)
    {
        return claimType;
    }

    public string GetFirstName()
    {
        return "FirstName";
    }

    public string GetLastName()
    {
        return "LastName";

    }

    public string GetUserAgent()
    {
        return "1";
    }

    public string GetUserIp()
    {
        return "127.0.0.1";
    }

    public string GetUsername()
    {
        return "UserName";
    }

    public bool HasAccess(string claimType,string value)
    {
        return true;
    }

    public bool IsCurrentUser(string userId)
    {
        return true;

    }

    public string UserId()
    {
        return "1";
    }

    public string UserIdOrDefault()
    {
        return "1";
    }

    public string UserIdOrDefault(string defaultValue)
    {
        return "1";
    }
}
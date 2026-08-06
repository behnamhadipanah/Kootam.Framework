# User Management Extension

Access current user information from HTTP context — claims, IP, user agent, and authorization checks.

| Package | Type | Path |
|---------|------|------|
| `Kootam.UserManagement.Abstractions` | Contracts + `FakeUserInfoService` | `Extensions/Kootam.Abstractions/Kootam.UserManagement.Abstractions/` |
| `Kootam.UserManagement` | Claim helper extensions | `Extensions/UserManagement/Kootam.UserManagement/src/Kootam.UserManagement/` |

Works alongside [Authentication](./authentication.md) — register auth first, then user info services.

---

## Abstraction

```csharp
public interface IUserInfoService
{
    string UserId();
    string UserIdOrDefault();
    string UserIdOrDefault(string defaultValue);
    string GetUsername();
    string GetFirstName();
    string GetLastName();
    string GetUserIp();
    string GetUserAgent();
    string? GetClaim(string claimType);
    bool IsCurrentUser(string userId);
    bool HasAccess(string claimType, string value);
}
```

---

## Registration

Implement `IUserInfoService` in your Infrastructure or API project:

```csharp
public class AppUserInfoService(IHttpContextAccessor accessor) : IUserInfoService
{
    private ClaimsPrincipal User => accessor.HttpContext!.User;

    public string UserId() => User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new UnauthorizedAccessException();
    public string GetUsername() => User.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty;
    public string GetFirstName() => User.FindFirst("given_name")?.Value ?? string.Empty;
    public string GetLastName() => User.FindFirst("family_name")?.Value ?? string.Empty;
    public string GetUserIp() => accessor.HttpContext!.Connection.RemoteIpAddress?.ToString() ?? string.Empty;
    public string GetUserAgent() => accessor.HttpContext!.Request.Headers.UserAgent.ToString();
    public string? GetClaim(string claimType) => User.FindFirst(claimType)?.Value;
    public bool IsCurrentUser(string userId) => UserId() == userId;
    public bool HasAccess(string claimType, string value) => User.HasClaim(claimType, value);
    public string UserIdOrDefault() => User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
    public string UserIdOrDefault(string defaultValue) => User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? defaultValue;
}

// Program.cs
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUserInfoService, AppUserInfoService>();
```

### Development / Testing

```csharp
builder.Services.AddScoped<IUserInfoService, FakeUserInfoService>();
```

`FakeUserInfoService` returns stub values — use in samples and integration tests.

---

## Claim Extensions

`Kootam.UserManagement` provides helper extensions on `ClaimsPrincipal`:

```csharp
using Kootam.UserManagement.DependencyInjection;

var role = User.GetClaim("role");
```

---

## Usage in Command Handlers

```csharp
public class UpdateProfileCommandHandler(IUserInfoService userInfo, ...)
    : IRequestHandler<UpdateProfileCommand>
{
    public async Task<Result> Handle(UpdateProfileCommand command, CancellationToken ct)
    {
        var currentUserId = userInfo.UserId();

        if (!userInfo.IsCurrentUser(command.UserId.ToString()))
            return Result.Failure(ResultStatus.Forbidden, "Cannot update another user's profile");

        // ...
        return Result.Success();
    }
}
```

---

## Integration with Authentication

Typical setup:

```csharp
builder.Services
    .AddKootamAuthentication("AppScheme")
    .UseCookies()
    .AddJwt(builder.Configuration);

builder.Services.AddCurrentUser();                              // ICurrentUserAccessor
builder.Services.AddScoped<IUserInfoService, AppUserInfoService>();
builder.Services.AddScoped<IUserClaimsMapper<AppUser>, AppUserClaimsMapper>();
```

| Service | Purpose |
|---------|---------|
| `ICurrentUserAccessor` | Low-level current user access (auth package) |
| `IUserInfoService` | Rich user info for business logic |
| `IUserClaimsMapper<TUser>` | Map user entity → claims at login |

---

## Agent Rules

- Inject `IUserInfoService` in handlers for user-scoped operations — do not read `HttpContext` directly in Application layer.
- Use `FakeUserInfoService` only in dev/test — implement real service for production.
- Authorization checks (`HasAccess`, `IsCurrentUser`) belong in handlers, not controllers.
- Claim type strings should use constants (e.g. `KootamClaimTypes`) where available.

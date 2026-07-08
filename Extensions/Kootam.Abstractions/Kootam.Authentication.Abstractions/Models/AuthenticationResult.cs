using System.Security.Claims;

namespace Kootam.Authentication.Abstractions.Models;

public class AuthenticationResult
{
    public bool Succeeded { get; set; }

    public ClaimsPrincipal? Principal { get; set; }

    public string? FailureMessage { get; set; }

    public static AuthenticationResult Success(ClaimsPrincipal principal)
        => new() { Succeeded = true, Principal = principal };

    public static AuthenticationResult Fail(string message)
        => new() { Succeeded = false, FailureMessage = message };
}
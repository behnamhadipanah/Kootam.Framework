using System.Security.Claims;

namespace Kootam.Extensions.Authentication.Abstractions.Models;

public class AuthResult
{
    public bool Succeeded { get; set; }

    public ClaimsPrincipal? Principal { get; set; }

    public string? FailureMessage { get; set; }

    public static AuthResult Success(ClaimsPrincipal principal)
        => new() { Succeeded = true, Principal = principal };

    public static AuthResult Fail(string message)
        => new() { Succeeded = false, FailureMessage = message };
}
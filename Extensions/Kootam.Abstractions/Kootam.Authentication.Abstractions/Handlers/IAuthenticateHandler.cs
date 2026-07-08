using Kootam.Authentication.Abstractions.Models;
using Microsoft.AspNetCore.Http;

namespace Kootam.Authentication.Abstractions.Handlers;

public interface IAuthenticateHandler
{
    string Scheme { get; }

    Task<AuthenticationResult> AuthenticateAsync(HttpContext context);
}

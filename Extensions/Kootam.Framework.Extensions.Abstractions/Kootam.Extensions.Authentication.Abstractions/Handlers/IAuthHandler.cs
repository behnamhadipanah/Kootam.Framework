using Kootam.Extensions.Authentication.Abstractions.Models;
using Microsoft.AspNetCore.Http;

namespace Kootam.Extensions.Authentication.Abstractions.Handlers;

public interface IAuthHandler
{
    string Scheme { get; }

    Task<AuthResult> AuthenticateAsync(HttpContext context);
}

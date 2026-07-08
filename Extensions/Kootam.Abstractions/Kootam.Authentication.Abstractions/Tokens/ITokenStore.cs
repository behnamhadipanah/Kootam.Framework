using Kootam.Authentication.Abstractions.Models;
using Kootam.Authentication.Abstractions.Options;
using Microsoft.AspNetCore.Http;

namespace Kootam.Authentication.Abstractions.Tokens;

public interface ITokenStore
{
    Task SignInAsync(HttpContext context, CredentialsOptions data);
    Task SignOutAsync(HttpContext context);
}
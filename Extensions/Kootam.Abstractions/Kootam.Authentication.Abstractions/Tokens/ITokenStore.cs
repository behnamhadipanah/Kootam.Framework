using Kootam.Authentication.Abstractions.Models;
using Kootam.Authentication.Abstractions.Options;
using Microsoft.AspNetCore.Http;

namespace Kootam.Authentication.Abstractions.Tokens;

public interface ITokenStore<TUserKey>
{
    Task SignInAsync(HttpContext context, TokenStoreOption<TUserKey> data);
    Task SignOutAsync(HttpContext context);
}
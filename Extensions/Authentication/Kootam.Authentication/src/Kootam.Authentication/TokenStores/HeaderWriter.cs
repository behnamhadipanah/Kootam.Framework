using Kootam.Authentication.Abstractions.Options;
using Kootam.Authentication.Abstractions.Tokens;
using Microsoft.AspNetCore.Http;

namespace Kootam.Authentication.TokenStores;

public sealed class HeaderWriter<TUserKey> : ITokenStore<TUserKey>
{
    public Task SignInAsync(HttpContext context, TokenStoreOption<TUserKey> data)
    {
        context.Response.Headers.Append("Authorization", $"Bearer {data.AccessToken}");

        return Task.CompletedTask;
    }

    public Task SignOutAsync(HttpContext context)
    {
        context.Response.Headers.Remove("Authorization");

        return Task.CompletedTask;
    }
}
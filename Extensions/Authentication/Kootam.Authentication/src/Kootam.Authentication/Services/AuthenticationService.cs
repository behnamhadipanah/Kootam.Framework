using Kootam.Authentication.Abstractions.Models;
using Kootam.Authentication.Abstractions.Options;
using Kootam.Authentication.Abstractions.Services;
using Kootam.Authentication.Abstractions.Tokens;
using Microsoft.AspNetCore.Http;

namespace Kootam.Authentication.Services;

public class AuthenticationService<TUserKey>(IHttpContextAccessor httpContextAccessor, ITokenStore<TUserKey> tokenStore) : IAuthenticationService<TUserKey>
{

    public async Task SignInAsync(IssuedToken<TUserKey> token, CancellationToken cancellationToken = new CancellationToken())
    {
        var context = httpContextAccessor.HttpContext
                      ?? throw new InvalidOperationException("HttpContext not found.");

        await tokenStore.SignInAsync(context, new TokenStoreOption<TUserKey>()
        {
            AccessToken = token.AccessToken,
            RefreshToken = token.RefreshToken,
            AccessTokenExpires = token.AccessTokenExpires,
            RefreshTokenExpires = token.RefreshTokenExpires,
        });
    }

    public async Task SignOutAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        var context =
            httpContextAccessor.HttpContext
            ?? throw new InvalidOperationException();

        await tokenStore.SignOutAsync(context);
    }
}

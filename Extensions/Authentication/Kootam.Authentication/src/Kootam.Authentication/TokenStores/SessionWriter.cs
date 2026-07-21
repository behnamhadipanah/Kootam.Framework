using Kootam.Authentication.Abstractions.Options;
using Kootam.Authentication.Abstractions.Tokens;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Kootam.Authentication.TokenStores;

public sealed class SessionWriter<TUserKey> : ITokenStore<TUserKey>
{
    private readonly AuthenticationTransportOption _options;

    public SessionWriter(IOptions<AuthenticationTransportOption> options)
    {
        _options = options.Value;
    }

    public Task SignInAsync(HttpContext context, TokenStoreOption<TUserKey> data)
    {
        context.Session.SetString( _options.AccessTokenKey,
            data.AccessToken);

        if (data.RefreshToken is not null)
        {
            context.Session.SetString(
                _options.RefreshTokenKey,
                data.RefreshToken.Token);
        }

        return Task.CompletedTask;
    }

    public Task SignOutAsync(HttpContext context)
    {
        context.Session.Remove(_options.AccessTokenKey);
        context.Session.Remove(_options.RefreshTokenKey);

        return Task.CompletedTask;
    }
}
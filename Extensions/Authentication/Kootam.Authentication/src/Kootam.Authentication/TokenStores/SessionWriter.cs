using Kootam.Authentication.Abstractions.Options;
using Kootam.Authentication.Abstractions.Tokens;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Kootam.Authentication.TokenStores;

public sealed class SessionWriter : ITokenStore
{
    private readonly AuthenticationTransportOptions _options;

    public SessionWriter(IOptions<AuthenticationTransportOptions> options)
    {
        _options = options.Value;
    }

    public Task SignInAsync(HttpContext context, CredentialsOptions data)
    {
        context.Session.SetString( _options.AccessTokenKey,
            data.AccessToken);

        if (!string.IsNullOrWhiteSpace(data.RefreshToken))
        {
            context.Session.SetString(
                _options.RefreshTokenKey,
                data.RefreshToken);
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
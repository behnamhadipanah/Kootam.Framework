using Kootam.Authentication.Abstractions.Options;
using Kootam.Authentication.Abstractions.Tokens;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Kootam.Authentication.TokenStores;

public sealed class CookieWriter : ITokenStore
{
    private readonly AuthenticationTransportOptions _options;

    public CookieWriter(IOptions<AuthenticationTransportOptions> options)
    {
        _options = options.Value;
    }

    public Task SignInAsync(HttpContext context, CredentialsOptions data)
    {
        context.Response.Cookies.Append(
            _options.AccessTokenKey,
            data.AccessToken,
            new CookieOptions
            {
                HttpOnly = _options.HttpOnly,
                Secure = _options.Secure,
                SameSite = _options.SameSite,
                Expires = data.AccessTokenExpires
            });

        if (!string.IsNullOrWhiteSpace(data.RefreshToken))
        {
            context.Response.Cookies.Append(
                _options.RefreshTokenKey,
                data.RefreshToken,
                new CookieOptions
                {
                    HttpOnly = _options.HttpOnly,
                    Secure = _options.Secure,
                    SameSite = _options.SameSite,
                    Expires = data.RefreshTokenExpires
                });
        }

        return Task.CompletedTask;
    }

    public Task SignOutAsync(HttpContext context)
    {
        context.Response.Cookies.Delete(_options.AccessTokenKey);
        context.Response.Cookies.Delete(_options.RefreshTokenKey);

        return Task.CompletedTask;
    }
}
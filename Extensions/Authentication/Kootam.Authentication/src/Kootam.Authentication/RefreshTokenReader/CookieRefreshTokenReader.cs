using Kootam.Authentication.Abstractions.Options;
using Kootam.Authentication.Abstractions.Tokens;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Kootam.Authentication.RefreshTokenReader;

public sealed class CookieRefreshTokenReader : IRefreshTokenReader
{
    private readonly AuthenticationTransportOption _options;

    public CookieRefreshTokenReader(IOptions<AuthenticationTransportOption> options)
    {
        _options = options.Value;
    }

    public ValueTask<string?> ReadAsync(HttpContext context)
    {
        context.Request.Cookies.TryGetValue(_options.RefreshTokenKey, out var token);

        return ValueTask.FromResult(token);
    }
}
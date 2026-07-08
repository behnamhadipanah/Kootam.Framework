using Kootam.Authentication.Abstractions.Options;
using Kootam.Authentication.Abstractions.Tokens;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Kootam.Authentication.TokenReaders;

public sealed class CookieReader:ITokenReader
{
    private readonly AuthenticationTransportOption _options;

    public CookieReader(IOptions<AuthenticationTransportOption> options)
    {
        _options = options.Value;
    }
    public ValueTask<string?> ReadAsync(HttpContext context)
    {
        context.Request.Cookies.TryGetValue(_options.AccessTokenKey,out var token);

        return ValueTask.FromResult(token);
    }
}
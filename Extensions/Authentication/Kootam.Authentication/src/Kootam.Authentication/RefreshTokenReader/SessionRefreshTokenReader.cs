using Kootam.Authentication.Abstractions.Options;
using Kootam.Authentication.Abstractions.Tokens;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Kootam.Authentication.RefreshTokenReader;

public sealed class SessionRefreshTokenReader : IRefreshTokenReader
{
    private readonly AuthenticationTransportOption _options;

    public SessionRefreshTokenReader(IOptions<AuthenticationTransportOption> options)
    {
        _options = options.Value;
    }

    public ValueTask<string?> ReadAsync(HttpContext context)
    {
        var token = context.Session.GetString(_options.RefreshTokenKey);

        return ValueTask.FromResult(token);
    }
}
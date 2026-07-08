using Kootam.Authentication.Abstractions.Options;
using Kootam.Authentication.Abstractions.Tokens;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Kootam.Authentication.TokenReaders;

public sealed class SessionReader : ITokenReader
{
    private readonly AuthenticationTransportOptions _options;

    public SessionReader(IOptions<AuthenticationTransportOptions> options)
    {
        _options = options.Value;
    }

    public ValueTask<string?> ReadAsync(HttpContext context)
    {
        var token = context.Session.GetString(
            _options.AccessTokenKey);

        return ValueTask.FromResult(token);
    }
}
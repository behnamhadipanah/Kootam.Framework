using Kootam.Authentication.Abstractions.Options;
using Kootam.Authentication.Abstractions.Tokens;
using Kootam.Authentication.Builder;
using Kootam.Authentication.TokenReaders;
using Kootam.Authentication.TokenStores;
using Microsoft.Extensions.DependencyInjection;

namespace Kootam.Authentication.DependencyInjection;

public static class CookieTransportExtensions
{
    public static AuthenticationBuilder UseCookies(
        this AuthenticationBuilder builder,
        Action<AuthenticationTransportOptions>? configure = null)
    {
        if (configure != null)
            builder.Services.Configure(configure);

        builder.Services.AddScoped<ITokenReader, CookieReader>();

        builder.Services.AddScoped<ITokenStore, CookieWriter>();

        return builder;
    }
}
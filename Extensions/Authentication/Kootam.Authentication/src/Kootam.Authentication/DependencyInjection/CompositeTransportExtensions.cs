using Kootam.Authentication.Abstractions.Tokens;
using Kootam.Authentication.Builder;
using Kootam.Authentication.RefreshTokenReader;
using Kootam.Authentication.TokenReaders;
using Kootam.Authentication.TokenStores;
using Microsoft.Extensions.DependencyInjection;

namespace Kootam.Authentication.DependencyInjection;

/// <summary>
/// use header and cookie readers
/// </summary>
public static class CompositeTransportExtensions
{
    public static AuthenticationBuilder UseComposite(
        this AuthenticationBuilder builder)
    {
        builder.Services.AddScoped<HeaderReader>();

        builder.Services.AddScoped<CookieReader>();

        builder.Services.AddScoped<IAccessTokenReader, CompositeReader>();
        builder.Services.AddScoped<IRefreshTokenReader, CompositeRefreshTokenReader>();

        builder.Services.AddScoped(typeof(ITokenStore<>),typeof(CookieWriter<>));

        return builder;
    }
}
using Kootam.Authentication.Abstractions.Tokens;
using Kootam.Authentication.Builder;
using Kootam.Authentication.RefreshTokenReader;
using Kootam.Authentication.TokenReaders;
using Kootam.Authentication.TokenStores;
using Microsoft.Extensions.DependencyInjection;

namespace Kootam.Authentication.DependencyInjection;

public static class SessionTransportExtensions
{
    public static AuthenticationBuilder UseSession(
        this AuthenticationBuilder builder)
    {
        builder.Services.AddScoped<IAccessTokenReader, SessionReader>();

        builder.Services.AddScoped<IRefreshTokenReader, SessionRefreshTokenReader>();

        builder.Services.AddScoped(typeof(ITokenStore<>), typeof(SessionWriter<>));

        return builder;
    }
}
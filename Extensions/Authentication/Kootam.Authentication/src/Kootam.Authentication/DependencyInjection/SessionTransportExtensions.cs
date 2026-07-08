using Kootam.Authentication.Abstractions.Tokens;
using Kootam.Authentication.Builder;
using Kootam.Authentication.TokenReaders;
using Kootam.Authentication.TokenStores;
using Microsoft.Extensions.DependencyInjection;

namespace Kootam.Authentication.DependencyInjection;

public static class SessionTransportExtensions
{
    public static AuthenticationBuilder UseSession(
        this AuthenticationBuilder builder)
    {
        builder.Services.AddScoped<ITokenReader, SessionReader>();

        builder.Services.AddScoped<ITokenStore, SessionWriter>();

        return builder;
    }
}
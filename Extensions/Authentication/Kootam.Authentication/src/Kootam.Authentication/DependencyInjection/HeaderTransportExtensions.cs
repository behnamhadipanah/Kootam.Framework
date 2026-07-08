using Kootam.Authentication.Abstractions.Tokens;
using Kootam.Authentication.Builder;
using Kootam.Authentication.TokenReaders;
using Microsoft.Extensions.DependencyInjection;

namespace Kootam.Authentication.DependencyInjection;

public static class HeaderTransportExtensions
{
    public static AuthenticationBuilder UseAuthorizationHeader(
        this AuthenticationBuilder builder)
    {
        builder.Services.AddScoped<ITokenReader, HeaderReader>();

        return builder;
    }
}
using Kootam.Authentication.Abstractions.Services;
using Kootam.Authentication.Builder;
using Kootam.Authentication.Services;
using Microsoft.AspNetCore.Builder.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace Kootam.Authentication.DependencyInjection;

public static class RefreshTokenExtensions
{
    public static AuthenticationBuilder EnableRefreshTokens( this AuthenticationBuilder builder)
    {
        builder.Services.AddScoped<IRefreshTokenService, DefaultRefreshTokenService>();
        return builder;
    }
}
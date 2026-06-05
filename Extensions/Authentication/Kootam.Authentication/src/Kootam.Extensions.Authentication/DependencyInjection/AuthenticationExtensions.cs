using Kootam.Extensions.Authentication.Abstractions.Services;
using Kootam.Extensions.Authentication.Handlers;
using Kootam.Extensions.Authentication.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AuthenticationBuilder = Kootam.Extensions.Authentication.Builder.AuthenticationBuilder;

namespace Kootam.Extensions.Authentication.DependencyInjection;

public static class AuthenticationExtensions
{
    public const string KootamScheme = "KootamScheme";

    public static AuthenticationBuilder AddKootamAuthentication(
        this IServiceCollection services)
    {
        RegisterCoreServices(services);

        services.AddAuthentication(KootamScheme)
            .AddScheme<AuthenticationSchemeOptions, KootamAuthenticationHandler>(KootamScheme, null);

        return new AuthenticationBuilder(services);
    }

    public static AuthenticationBuilder AddKootamAuthentication(
        this IServiceCollection services, string scheme)
    {
        RegisterCoreServices(services);
        services.AddAuthentication(scheme)
            .AddScheme<AuthenticationSchemeOptions, KootamAuthenticationHandler>(scheme, null);
        return new AuthenticationBuilder(services);
    }

    public static AuthenticationBuilder AddKootamAuthentication(
        this IServiceCollection services, IConfiguration configuration)
    {
        RegisterCoreServices(services);
        var scheme = configuration.GetSection("AuthScheme").Value;
        services.AddAuthentication(scheme)
            .AddScheme<AuthenticationSchemeOptions, KootamAuthenticationHandler>(scheme, null);
        return new AuthenticationBuilder(services);
    }

    private static void RegisterCoreServices(IServiceCollection services)
    {
        services.AddHttpContextAccessor();

        services.AddScoped<IPasswordHasherService, PasswordHasherService>();
    }
}
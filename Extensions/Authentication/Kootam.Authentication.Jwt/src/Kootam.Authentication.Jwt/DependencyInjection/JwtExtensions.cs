using Kootam.Authentication.Abstractions.Handlers;
using Kootam.Authentication.Abstractions.Tokens;
using Kootam.Authentication.Builder;
using Kootam.Authentication.Jwt.Handlers;
using Kootam.Authentication.Jwt.Options;
using Kootam.Authentication.Jwt.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Kootam.Authentication.Jwt.DependencyInjection;

public static class JwtExtensions
{
    public static AuthenticationBuilder AddJwt<TUser>(
          this AuthenticationBuilder builder,
          IConfiguration configuration
        )
    {
        builder.Services
            .AddOptions<JwtOptions>()
            .Bind(configuration.GetSection("Jwt"))
            .ValidateDataAnnotations()                   
            .Validate(opt => !string.IsNullOrWhiteSpace(opt.Key),
                "JwtOptions.Key must not be empty.")
            .ValidateOnStart();                     

        RegisterCoreServices<TUser>(builder.Services);

        return builder;
    }
    public static AuthenticationBuilder AddJwt<TUser>(
        this AuthenticationBuilder builder,Action<JwtOptions> configure)
    {
        
        builder.Services
            .AddOptions<JwtOptions>()
            .Configure(configure)
            .ValidateDataAnnotations()
            .Validate(opt => !string.IsNullOrWhiteSpace(opt.Key),
                "JwtOptions.Key must not be empty.")
            .ValidateOnStart();

        RegisterCoreServices<TUser>(builder.Services);

        return builder;
    }
    private static void RegisterCoreServices<TUser>(IServiceCollection services)
    {
        services.AddScoped<ITokenValidator, JwtTokenValidator>();

        services.AddScoped<IAuthHandler, JwtAuthHandler>();

        services.AddScoped<ITokenGenerator<TUser>, JwtTokenGenerator<TUser>>();
    }
}

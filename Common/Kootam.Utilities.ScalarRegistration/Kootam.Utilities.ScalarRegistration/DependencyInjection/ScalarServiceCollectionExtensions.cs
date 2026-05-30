using Kootam.Utilities.ScalarRegistration.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Scalar.AspNetCore;

namespace Kootam.Utilities.ScalarRegistration.DependencyInjection;


public static class ScalarServiceCollectionExtensions
{
    public static IServiceCollection AddScalar(
        this IServiceCollection services,
        IConfiguration configuration,
        string sectionName)
    {
        return services.AddScalar(configuration.GetSection(sectionName));
    }

    public static IServiceCollection AddScalar(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<ScalarOption>(configuration);

        var option = configuration.Get<ScalarOption>() ?? new ScalarOption();
        return services.AddScalarInternal(option);
    }
    public static IServiceCollection AddScalar(this IServiceCollection services)
    {
        var option = new ScalarOption(); 
        services.Configure<ScalarOption>(_ => { });
        return services.AddScalarInternal(option);
    }
    public static IServiceCollection AddScalar(
        this IServiceCollection services,
        Action<ScalarOption> configure)
    {
        services.Configure(configure);

        var option = new ScalarOption();
        configure(option);

        return services.AddScalarInternal(option);
    }

    private static IServiceCollection AddScalarInternal(
        this IServiceCollection services,
        ScalarOption option)
    {
        if (option.Enabled)
            services.AddOpenApi();

        return services;
    }
    
    public static void UseScalar(this WebApplication app)
    {
        var option = app.Services.GetRequiredService<IOptions<ScalarOption>>().Value;
        if (option.Enabled)
        {
            app.MapOpenApi();
            app.MapScalarApiReference();
        }
    }
}

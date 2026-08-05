using Kootam.Translator.Abstractions;
using Kootam.Translator.Database.Database;
using Kootam.Translator.Database.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using TranslatorService = Kootam.Translator.Database.Services.Translator;

namespace Kootam.Translator.Database.DependencyInjection;

public static class TranslatorServiceCollectionExtensions
{
    public static IServiceCollection AddDbTranslator(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        return services.AddDbTranslator(
            configuration.GetSection(TranslatorOptions.DefaultTranslatorOptionsName));
    }

    public static IServiceCollection AddDbTranslator(
        this IServiceCollection services,
        IConfiguration configuration,
        string sectionName)
    {
        return services.AddDbTranslator(
            configuration.GetSection(sectionName));
    }

    public static IServiceCollection AddDbTranslator(
        this IServiceCollection services,
        IConfigurationSection section)
    {
        services.AddOptions<TranslatorOptions>()
            .Bind(section)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        RegisterCoreServices(services);
        return services;
    }

    public static IServiceCollection AddDbTranslator(
        this IServiceCollection services,
        Action<TranslatorOptions> setupAction)
    {
        services.AddOptions<TranslatorOptions>()
            .Configure(setupAction)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        RegisterCoreServices(services);
        return services;
    }

    private static void RegisterCoreServices(IServiceCollection services)
    {
        services.AddSingleton<IDbConnectionFactory>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<TranslatorOptions>>().Value;
            return new SqlConnectionFactory(options.ConnectionString);
        });

        services.AddSingleton<ITranslationStore, SqlDapperRepository>();
        services.AddSingleton<ITranslator, TranslatorService>();
    }
}

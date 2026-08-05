using Kootam.Translator.Abstractions;
using Kootam.Translator.Json.Options;
using Kootam.Translator.Json.Store;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using TranslatorService = Kootam.Translator.Json.Services.Translator;

namespace Kootam.Translator.Json.DependencyInjection;

public static class TranslatorServiceCollectionExtensions
{
    public static IServiceCollection AddJsonTranslator(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        return services.AddJsonTranslator(
            configuration.GetSection(JsonTranslatorOptions.DefaultSectionName));
    }

    public static IServiceCollection AddJsonTranslator(
        this IServiceCollection services,
        IConfiguration configuration,
        string sectionName)
    {
        return services.AddJsonTranslator(
            configuration.GetSection(sectionName));
    }

    public static IServiceCollection AddJsonTranslator(
        this IServiceCollection services,
        IConfigurationSection section)
    {
        services.AddOptions<JsonTranslatorOptions>()
            .Bind(section)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        RegisterCoreServices(services);
        return services;
    }

    public static IServiceCollection AddJsonTranslator(
        this IServiceCollection services,
        Action<JsonTranslatorOptions> setupAction)
    {
        services.AddOptions<JsonTranslatorOptions>()
            .Configure(setupAction)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        RegisterCoreServices(services);
        return services;
    }

    private static void RegisterCoreServices(IServiceCollection services)
    {
        services.AddSingleton<ITranslationStore, JsonTranslationStore>();
        services.AddSingleton<ITranslator, TranslatorService>();
    }
}

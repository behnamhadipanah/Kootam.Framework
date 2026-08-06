using Kootam.Translator.Abstractions;
using Kootam.Translator.Database.Builder;
using Kootam.Translator.Database.Database;
using Kootam.Translator.Database.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using TranslatorService = Kootam.Translator.Database.Services.Translator;

namespace Kootam.Translator.Database.DependencyInjection;

public static class TranslatorServiceCollectionExtensions
{
    public static TranslatorBuilder AddTranslator(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        return services.AddTranslator(
            configuration.GetSection(TranslatorOptions.DefaultTranslatorOptionsName));
    }

    public static TranslatorBuilder AddTranslator(
        this IServiceCollection services,
        IConfiguration configuration,
        string sectionName)
    {
        return services.AddTranslator(
            configuration.GetSection(sectionName));
    }

    public static TranslatorBuilder AddTranslator(
        this IServiceCollection services,
        IConfigurationSection section)
    {
        services.AddOptions<TranslatorOptions>()
                .Bind(section)
                .ValidateOnStart();

        RegisterTranslatorServices(services);

        return new TranslatorBuilder(services);
    }

    public static TranslatorBuilder AddTranslator(
        this IServiceCollection services,
        Action<TranslatorOptions> setupAction)
    {
        services.Configure(setupAction);

        RegisterTranslatorServices(services);

        return new TranslatorBuilder(services);
    }

    private static void RegisterTranslatorServices(IServiceCollection services)
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

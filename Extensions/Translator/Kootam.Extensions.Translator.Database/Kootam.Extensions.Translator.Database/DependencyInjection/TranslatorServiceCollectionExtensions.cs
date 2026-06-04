using Kootam.Extensions.Translator.Abstractions;
using Kootam.Extensions.Translator.Database.Database;
using Kootam.Extensions.Translator.Database.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using TranslatorService = Kootam.Extensions.Translator.Database.Services.Translator;

namespace Kootam.Extensions.Translator.Database.DependencyInjection;

public static class TranslatorServiceCollectionExtensions
{
    public static IServiceCollection AddTranslator(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        return services.AddTranslator(
            configuration.GetSection(TranslatorOptions.DefaultTranslatorOptionsName));
    }

    public static IServiceCollection AddTranslator(
        this IServiceCollection services,
        IConfiguration configuration,
        string sectionName)
    {
        return services.AddTranslator(
            configuration.GetSection(sectionName));
    }

    public static IServiceCollection AddTranslator(
        this IServiceCollection services,
        IConfigurationSection section)
    {
        services.AddSingleton<ITranslator, TranslatorService>();

        //services.AddOptions<TranslatorOptions>()
        //        .Bind(section)
        //        .ValidateDataAnnotations()
        //        .ValidateOnStart();

        services.AddSingleton<IDbConnectionFactory>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<TranslatorOptions>>().Value;
            return new SqlConnectionFactory(options.ConnectionString);
        });

        services.AddSingleton<ITranslator, SqlDapperRepository>();

        return services;
    }

    public static IServiceCollection AddTranslator(
        this IServiceCollection services,
        Action<TranslatorOptions> setupAction)
    {
        services.AddSingleton<ITranslator, TranslatorService>();

        services.Configure(setupAction);

        services.AddSingleton<IDbConnectionFactory>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<TranslatorOptions>>().Value;
            return new SqlConnectionFactory(options.ConnectionString);
        });

        services.AddSingleton<ITranslator, SqlDapperRepository>();

        return services;
    }
}

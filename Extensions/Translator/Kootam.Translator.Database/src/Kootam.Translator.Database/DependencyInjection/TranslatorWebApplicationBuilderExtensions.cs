using Kootam.Translator.Database.Builder;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Kootam.Translator.Database.DependencyInjection;

public static class TranslatorWebApplicationBuilderExtensions
{
    public static TranslatorBuilder AddTranslator(
        this WebApplicationBuilder builder,
        IConfiguration? configuration = null)
    {
        return builder.Services.AddTranslator(configuration ?? builder.Configuration);
    }

    public static TranslatorBuilder AddTranslator(
        this WebApplicationBuilder builder,
        Action<Options.TranslatorOptions> setupAction)
    {
        return builder.Services.AddTranslator(setupAction);
    }
}

using Kootam.Translator.Database.Builder;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Kootam.Translator.Database.DependencyInjection;

public static class TranslatorWebApplicationBuilderExtensions
{
    public static TranslatorBuilder AddDbTranslator(
        this WebApplicationBuilder builder,
        IConfiguration? configuration = null)
    {
        return builder.Services.AddDbTranslator(configuration ?? builder.Configuration);
    }

    public static TranslatorBuilder AddDbTranslator(
        this WebApplicationBuilder builder,
        Action<Options.TranslatorOptions> setupAction)
    {
        return builder.Services.AddDbTranslator(setupAction);
    }
}

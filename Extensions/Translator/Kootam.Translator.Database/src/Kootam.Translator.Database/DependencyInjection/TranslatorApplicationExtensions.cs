using Kootam.Translator.Abstractions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Kootam.Translator.Database.DependencyInjection;

public static class TranslatorApplicationExtensions
{
    public static WebApplication UseTranslator(this WebApplication app)
    {
        _ = app.Services.GetRequiredService<ITranslationStore>();
        return app;
    }
}

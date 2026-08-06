using Kootam.Translator.Database.Options;
using Microsoft.Extensions.DependencyInjection;

namespace Kootam.Translator.Database.Builder;

public sealed class TranslatorBuilder
{
    internal TranslatorBuilder(IServiceCollection services)
    {
        Services = services;
    }

    public IServiceCollection Services { get; }

    public TranslatorBuilder UseCaching(int reloadIntervalInMinutes = 5)
    {
        Services.PostConfigure<TranslatorOptions>(options =>
        {
            options.UseCaching = true;
            options.ReloadDataIntervalInMinuts = reloadIntervalInMinutes;
        });

        return this;
    }

    public TranslatorBuilder WithoutCaching()
    {
        Services.PostConfigure<TranslatorOptions>(options => options.UseCaching = false);
        return this;
    }

    public TranslatorBuilder AutoCreateTable(bool enabled = true)
    {
        Services.PostConfigure<TranslatorOptions>(options => options.AutoCreateSqlTable = enabled);
        return this;
    }

    public TranslatorBuilder UseMigrations()
    {
        Services.PostConfigure<TranslatorOptions>(options => options.AutoCreateSqlTable = false);
        return this;
    }
}

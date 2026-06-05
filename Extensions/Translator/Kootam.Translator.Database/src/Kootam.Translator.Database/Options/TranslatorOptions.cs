using System.ComponentModel.DataAnnotations;

namespace Kootam.Translator.Database.Options;

public class TranslatorOptions
{
    public static string DefaultTranslatorOptionsName = "Translator";
    public string ConnectionString { get; set; } = string.Empty;
    public bool AutoCreateSqlTable { get; set; } = true;
    public string TableName { get; set; } = "Translations";
    public string SchemaName { get; set; } = "dbo";
    public int ReloadDataIntervalInMinuts { get; set; }
    public DefaultTranslationOption[] DefaultTranslations { get; set; } = Array.Empty<DefaultTranslationOption>();

    [Required]
    public string DefaultCulture { get; set; } = "fa-IR";

    public string FallbackCulture { get; set; } = "en-US";

}

public class DefaultTranslationOption
{
    public string Key { get; set; } = nameof(Key);
    public string Value { get; set; } = nameof(Value);
    public string Culture { get; set; } = nameof(Culture);
}
using System.ComponentModel.DataAnnotations;

namespace Kootam.Translator.Json.Options;

public class JsonTranslatorOptions
{
    public const string DefaultSectionName = "Translator";

    [Required]
    public string FilePath { get; set; } = "translations.json";

    public int ReloadDataIntervalInMinuts { get; set; }

    [Required]
    public string DefaultCulture { get; set; } = "fa-IR";

    public string FallbackCulture { get; set; } = "en-US";

    public JsonTranslationEntry[] DefaultTranslations { get; set; } = [];
}

public class JsonTranslationEntry
{
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string Culture { get; set; } = string.Empty;
}

namespace Kootam.Translator.Json.Models;

internal sealed class JsonTranslationFile
{
    public JsonTranslationRecord[] Translations { get; set; } = [];
}

internal sealed class JsonTranslationRecord
{
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string Culture { get; set; } = string.Empty;
}

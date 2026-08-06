using Kootam.Translator.Abstractions;
using System.Globalization;

namespace Kootam.Translator.Json.Services;

public sealed class Translator(ITranslationStore store) : ITranslator
{
    private CultureInfo Culture => CultureInfo.CurrentUICulture;

    public string this[string key] => Get(key);

    public string this[string key, params object[] arguments] => Get(key, arguments);

    public string Get(string key) => store.Get(key, Culture.Name);

    public string Get(string key, params object[] arguments)
    {
        var pattern = Get(key);

        return arguments is null || arguments.Length == 0
            ? pattern
            : string.Format(Culture, pattern, arguments);
    }

    public string Get(string key, CultureInfo culture)
    {
        var cultureName = culture?.Name ?? Culture.Name;
        return store.Get(key, cultureName);
    }

    public string Get(string key, CultureInfo culture, params object[] arguments)
    {
        var pattern = Get(key, culture);

        return arguments is null || arguments.Length == 0
            ? pattern
            : string.Format(culture ?? Culture, pattern, arguments);
    }

    public string GetConcateString(char separator, params string[] keys)
        => string.Join(separator, keys.Select(k => Get(k)));
}

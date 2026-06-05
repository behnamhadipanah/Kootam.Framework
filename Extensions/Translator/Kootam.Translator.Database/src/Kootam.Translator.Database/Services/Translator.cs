using Microsoft.Extensions.Logging;
using System.Globalization;
using Kootam.Translator.Abstractions;

namespace Kootam.Translator.Database.Services;

public sealed class Translator(ITranslator store, ILogger<Translator> logger) : ITranslator
{
    private CultureInfo Culture => CultureInfo.CurrentUICulture;
    #region Indexers 
    public string this[string key]
        => Get(key);

    public string this[string key, params object[] arguments]
        => Get(key, arguments);

    public string this[CultureInfo culture, string key]
        => Get(key, culture);
    #endregion

    #region Implementation

    public string Get(string key)
    {
        return store.Get(key, Culture.Name);
    }

    public string Get(string key, params object[] arguments)
    {
        var pattern = Get(key);

        return arguments == null || arguments.Length == 0
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

        return arguments == null || arguments.Length == 0
            ? pattern
            : string.Format(
                culture ?? Culture,
                pattern,
                arguments);
    }
    #endregion

    public string GetConcateString(char separator, params string[] keys)
        => string.Join(separator, keys.Select(k => Get(k)));
}



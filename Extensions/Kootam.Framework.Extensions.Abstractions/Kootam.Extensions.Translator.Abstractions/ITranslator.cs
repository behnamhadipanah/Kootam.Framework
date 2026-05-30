using System.Globalization;

namespace Kootam.Extensions.Translator.Abstractions;

public interface ITranslator
{
    string this[string key] { get; }

    string this[string key, params object[] arguments] { get; }

    string Get(string key);

    string Get(string key, params object[] arguments);

    string Get(string key, CultureInfo culture);

    string Get(string key, CultureInfo culture, params object[] arguments);
}
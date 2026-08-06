namespace Kootam.Translator.Abstractions;

public interface ITranslationStore
{
    string Get(string key, string culture);
}

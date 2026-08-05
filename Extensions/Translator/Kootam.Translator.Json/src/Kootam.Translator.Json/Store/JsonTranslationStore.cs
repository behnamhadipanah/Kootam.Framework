using Kootam.Translator.Abstractions;
using Kootam.Translator.Json.Models;
using Kootam.Translator.Json.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using System.Text.Json;

namespace Kootam.Translator.Json.Store;

public sealed class JsonTranslationStore : ITranslationStore, IDisposable
{
    private readonly JsonTranslatorOptions _configuration;
    private readonly ILogger<JsonTranslationStore> _logger;
    private readonly ConcurrentDictionary<(string Key, string Culture), string> _cache = new();
    private readonly Timer? _reloadTimer;
    private bool _disposed;

    public JsonTranslationStore(
        IOptions<JsonTranslatorOptions> options,
        ILogger<JsonTranslationStore> logger)
    {
        _configuration = options.Value;
        _logger = logger;

        LoadTranslations();
        MergeDefaultTranslations();

        if (_configuration.ReloadDataIntervalInMinuts > 0)
        {
            var interval = TimeSpan.FromMinutes(_configuration.ReloadDataIntervalInMinuts);
            _reloadTimer = new Timer(_ => ReloadTranslations(), null, interval, interval);
        }
    }

    public string Get(string key, string culture)
    {
        if (TryGetCachedValue(key, culture, out var value))
            return value;

        if (TryGetFallbackValue(key, culture, out value))
            return value;

        _logger.LogWarning(
            "Missing translation. Key: {Key}, Culture: {Culture}",
            key,
            culture);

        return key;
    }

    private bool TryGetCachedValue(string key, string culture, out string value)
        => _cache.TryGetValue((key, culture), out value!);

    private bool TryGetFallbackValue(string key, string culture, out string value)
    {
        value = string.Empty;

        if (string.IsNullOrWhiteSpace(_configuration.FallbackCulture)
            || string.Equals(culture, _configuration.FallbackCulture, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        return TryGetCachedValue(key, _configuration.FallbackCulture, out value);
    }

    private void ReloadTranslations()
    {
        LoadTranslations();
        MergeDefaultTranslations();
    }

    private void LoadTranslations()
    {
        try
        {
            var filePath = ResolveFilePath(_configuration.FilePath);
            if (!File.Exists(filePath))
            {
                _logger.LogWarning("Translation file not found at {FilePath}", filePath);
                return;
            }

            var json = File.ReadAllText(filePath);
            var records = ParseTranslations(json);
            var updated = new ConcurrentDictionary<(string Key, string Culture), string>();

            foreach (var record in records)
                updated[(record.Key, record.Culture)] = record.Value;

            _cache.Clear();
            foreach (var pair in updated)
                _cache[pair.Key] = pair.Value;

            _logger.LogInformation(
                "Loaded {Count} translation records from {FilePath}",
                _cache.Count,
                filePath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Loading translations from JSON failed");
        }
    }

    private void MergeDefaultTranslations()
    {
        try
        {
            foreach (var item in _configuration.DefaultTranslations)
            {
                _cache.TryAdd((item.Key, item.Culture), item.Value);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Merging default translations failed");
        }
    }

    private static IEnumerable<JsonTranslationRecord> ParseTranslations(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return [];

        using var document = JsonDocument.Parse(json);

        return document.RootElement.ValueKind switch
        {
            JsonValueKind.Array => document.RootElement
                .EnumerateArray()
                .Select(ReadTranslationRecord)
                .Where(record => !string.IsNullOrWhiteSpace(record.Key)),

            JsonValueKind.Object when document.RootElement.TryGetProperty("translations", out var translations)
                => translations
                    .EnumerateArray()
                    .Select(ReadTranslationRecord)
                    .Where(record => !string.IsNullOrWhiteSpace(record.Key)),

            _ => JsonSerializer.Deserialize<JsonTranslationFile>(json)?.Translations ?? []
        };
    }

    private static JsonTranslationRecord ReadTranslationRecord(JsonElement element)
        => new()
        {
            Key = element.TryGetProperty("Key", out var key)
                ? key.GetString() ?? string.Empty
                : element.TryGetProperty("key", out key)
                    ? key.GetString() ?? string.Empty
                    : string.Empty,
            Value = element.TryGetProperty("Value", out var value)
                ? value.GetString() ?? string.Empty
                : element.TryGetProperty("value", out value)
                    ? value.GetString() ?? string.Empty
                    : string.Empty,
            Culture = element.TryGetProperty("Culture", out var culture)
                ? culture.GetString() ?? string.Empty
                : element.TryGetProperty("culture", out culture)
                    ? culture.GetString() ?? string.Empty
                    : string.Empty
        };

    private static string ResolveFilePath(string configuredPath)
        => Path.IsPathRooted(configuredPath)
            ? configuredPath
            : Path.Combine(AppContext.BaseDirectory, configuredPath);

    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;
        _reloadTimer?.Dispose();
    }
}

using Dapper;
using Kootam.Translator.Abstractions;
using Kootam.Translator.Database.Models;
using Kootam.Translator.Database.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;

namespace Kootam.Translator.Database.Database;

public sealed class SqlDapperRepository : ITranslationStore, IDisposable
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly TranslatorOptions _configuration;
    private readonly ILogger<SqlDapperRepository> _logger;

    private readonly ConcurrentDictionary<(string Key, string Culture), LocalizationRecord> _cache = new();
    private readonly Timer? _reloadTimer;

    private readonly string _selectAllCommand;
    private readonly string _selectByKeyCommand;
    private readonly string _insertCommand;

    private bool _disposed;

    public SqlDapperRepository(
        IDbConnectionFactory connectionFactory,
        IOptions<TranslatorOptions> options,
        ILogger<SqlDapperRepository> logger)
    {
        _connectionFactory = connectionFactory;
        _configuration = options.Value;
        _logger = logger;

        _selectAllCommand =
            $"SELECT * FROM [{_configuration.SchemaName}].[{_configuration.TableName}]";

        _selectByKeyCommand =
            $"SELECT TOP 1 * FROM [{_configuration.SchemaName}].[{_configuration.TableName}] " +
            $"WHERE [Key] = @Key AND [Culture] = @Culture";

        _insertCommand =
            $"INSERT INTO [{_configuration.SchemaName}].[{_configuration.TableName}]([Key],[Value],[Culture]) " +
            $"VALUES (@Key,@Value,@Culture); SELECT CAST(SCOPE_IDENTITY() as bigint);";

        TranslatorDatabaseInitializer.EnsureTable(_connectionFactory, _configuration, _logger);

        if (_configuration.UseCaching)
        {
            LoadLocalizationRecords();
            TranslatorDatabaseInitializer.SeedDefaults(
                _connectionFactory,
                _configuration,
                _logger,
                _cache);
            LoadLocalizationRecords();

            if (_configuration.ReloadDataIntervalInMinuts > 0)
            {
                var interval = TimeSpan.FromMinutes(_configuration.ReloadDataIntervalInMinuts);
                _reloadTimer = new Timer(_ => LoadLocalizationRecords(), null, interval, interval);
            }
        }
        else
        {
            TranslatorDatabaseInitializer.SeedDefaults(
                _connectionFactory,
                _configuration,
                _logger);
        }
    }

    public string Get(string key, string culture)
        => _configuration.UseCaching
            ? GetFromCache(key, culture)
            : GetFromDatabase(key, culture);

    private string GetFromCache(string key, string culture)
    {
        if (TryGetCachedValue(key, culture, out var value))
            return value;

        if (TryGetFallbackValue(key, culture, out value))
            return value;

        return InsertMissingTranslation(key, culture);
    }

    private string GetFromDatabase(string key, string culture)
    {
        var value = QueryTranslation(key, culture);
        if (value is not null)
            return value;

        if (!string.IsNullOrWhiteSpace(_configuration.FallbackCulture)
            && !string.Equals(culture, _configuration.FallbackCulture, StringComparison.OrdinalIgnoreCase))
        {
            value = QueryTranslation(key, _configuration.FallbackCulture);
            if (value is not null)
                return value;
        }

        return InsertMissingTranslation(key, culture);
    }

    private string? QueryTranslation(string key, string culture)
    {
        try
        {
            using var connection = _connectionFactory.CreateConnection();
            var record = connection.QueryFirstOrDefault<LocalizationRecord>(
                _selectByKeyCommand,
                new { Key = key, Culture = culture });

            return record?.Value;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Read translation failed. Key: {Key}, Culture: {Culture}", key, culture);
            return null;
        }
    }

    private bool TryGetCachedValue(string key, string culture, out string value)
    {
        if (_cache.TryGetValue((key, culture), out var record))
        {
            value = record.Value;
            return true;
        }

        value = string.Empty;
        return false;
    }

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

    private string InsertMissingTranslation(string key, string culture)
    {
        var record = new LocalizationRecord
        {
            Key = key,
            Culture = culture,
            Value = key
        };

        try
        {
            using var connection = _connectionFactory.CreateConnection();
            record.Id = connection.Query<long>(_insertCommand, record).FirstOrDefault();

            if (_configuration.UseCaching)
                _cache[(key, culture)] = record;

            _logger.LogInformation(
                "Missing translation inserted. Key: {Key}, Culture: {Culture}",
                key,
                culture);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Insert missing translation failed. Key: {Key}, Culture: {Culture}", key, culture);
        }

        return record.Value;
    }

    private void LoadLocalizationRecords()
    {
        try
        {
            _logger.LogInformation("Translator loading records...");

            using var connection = _connectionFactory.CreateConnection();
            var records = connection.Query<LocalizationRecord>(_selectAllCommand).ToList();

            var updated = new ConcurrentDictionary<(string Key, string Culture), LocalizationRecord>();
            foreach (var record in records)
                updated[(record.Key, record.Culture)] = record;

            _cache.Clear();
            foreach (var pair in updated)
                _cache[pair.Key] = pair.Value;

            _logger.LogInformation("Loaded {Count} translation records", _cache.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Loading localization records failed");
        }
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;
        _reloadTimer?.Dispose();
    }
}

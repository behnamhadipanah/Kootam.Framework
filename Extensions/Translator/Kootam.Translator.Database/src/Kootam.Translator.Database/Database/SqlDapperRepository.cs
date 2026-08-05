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

    private readonly string _selectCommand;
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

        _selectCommand =
            $"SELECT * FROM [{_configuration.SchemaName}].[{_configuration.TableName}]";

        _insertCommand =
            $"INSERT INTO [{_configuration.SchemaName}].[{_configuration.TableName}]([Key],[Value],[Culture]) " +
            $"VALUES (@Key,@Value,@Culture); SELECT CAST(SCOPE_IDENTITY() as bigint);";

        if (_configuration.AutoCreateSqlTable)
            CreateTableIfNeeded();

        LoadLocalizationRecords();
        SeedData();
        LoadLocalizationRecords();

        if (_configuration.ReloadDataIntervalInMinuts > 0)
        {
            var interval = TimeSpan.FromMinutes(_configuration.ReloadDataIntervalInMinuts);
            _reloadTimer = new Timer(_ => LoadLocalizationRecords(), null, interval, interval);
        }
    }

    public string Get(string key, string culture)
    {
        if (TryGetCachedValue(key, culture, out var value))
            return value;

        if (TryGetFallbackValue(key, culture, out value))
            return value;

        return InsertMissingTranslation(key, culture);
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

    private void CreateTableIfNeeded()
    {
        try
        {
            using var connection = _connectionFactory.CreateConnection();

            var sql = $@"
IF NOT EXISTS (
    SELECT * FROM INFORMATION_SCHEMA.TABLES
    WHERE TABLE_SCHEMA = '{_configuration.SchemaName}'
      AND TABLE_NAME = '{_configuration.TableName}'
)
BEGIN
    CREATE TABLE [{_configuration.SchemaName}].[{_configuration.TableName}](
        Id BIGINT IDENTITY(1,1) PRIMARY KEY,
        BusinessId UNIQUEIDENTIFIER NOT NULL UNIQUE DEFAULT NEWID(),
        [Key] NVARCHAR(255) NOT NULL,
        [Value] NVARCHAR(500) NOT NULL,
        [Culture] NVARCHAR(5) NULL
    )
END";

            connection.Execute(sql);

            _logger.LogInformation("Translator table ensured.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Create table failed");
            throw;
        }
    }

    private void LoadLocalizationRecords()
    {
        try
        {
            _logger.LogInformation("Translator loading records...");

            using var connection = _connectionFactory.CreateConnection();
            var records = connection.Query<LocalizationRecord>(_selectCommand).ToList();

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

    private void SeedData()
    {
        try
        {
            var missing = _configuration.DefaultTranslations
                .Where(d => !_cache.ContainsKey((d.Key, d.Culture)))
                .ToList();

            if (missing.Count == 0)
                return;

            using var connection = _connectionFactory.CreateConnection();

            foreach (var item in missing)
            {
                connection.Execute(_insertCommand, item);
                _cache[(item.Key, item.Culture)] = new LocalizationRecord
                {
                    Key = item.Key,
                    Culture = item.Culture,
                    Value = item.Value
                };
            }

            _logger.LogInformation("Seeded {Count} default translations", missing.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SeedData failed");
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

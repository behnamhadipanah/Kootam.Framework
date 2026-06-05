using Dapper;
using Kootam.Translator.Abstractions;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Globalization;
using Kootam.Translator.Database.Models;
using Kootam.Translator.Database.Options;

namespace Kootam.Translator.Database.Database;

public class SqlDapperRepository : ITranslator
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly TranslatorOptions _configuration;
    private readonly ILogger<SqlDapperRepository> _logger;

    private readonly ConcurrentDictionary<(string Key, string Culture), LocalizationRecord> _cache = new();
    private readonly Timer _reloadTimer;

    private readonly string _selectCommand;
    private readonly string _insertCommand;

    private bool _disposed;

    public SqlDapperRepository(
        IDbConnectionFactory connectionFactory,
        TranslatorOptions configuration,
        ILogger<SqlDapperRepository> logger)
    {
        _connectionFactory = connectionFactory;
        _configuration = configuration;
        _logger = logger;

        _selectCommand =
            $"SELECT * FROM [{configuration.SchemaName}].[{configuration.TableName}]";

        _insertCommand =
            $"INSERT INTO [{configuration.SchemaName}].[{configuration.TableName}]([Key],[Value],[Culture]) " +
            $"VALUES (@Key,@Value,@Culture); SELECT CAST(SCOPE_IDENTITY() as bigint);";

        if (_configuration.AutoCreateSqlTable)
            CreateTableIfNeeded();

        LoadLocalizationRecords();
        SeedData();
        LoadLocalizationRecords();

        _reloadTimer = new Timer(
            _ => LoadLocalizationRecords(),
            null,
            TimeSpan.FromMinutes(configuration.ReloadDataIntervalInMinuts),
            TimeSpan.FromMinutes(configuration.ReloadDataIntervalInMinuts));
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
            var records = connection.Query<LocalizationRecord>(_selectCommand);

            _cache.Clear();
            foreach (var record in records)
                _cache[(record.Key, record.Culture)] = record;

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

            if (!missing.Any()) return;

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

    public string Get(string key, string culture)
    {
        if (_cache.TryGetValue((key, culture), out var record))
            return record.Value;

        record = new LocalizationRecord
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
                key, culture);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Insert missing translation failed");
        }

        return record.Value;
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _reloadTimer.Dispose();
    }

    public string Get(string key)
    {
        throw new NotImplementedException();
    }

    public string Get(string key, params object[] arguments)
    {
        throw new NotImplementedException();
    }

    public string Get(string key, CultureInfo culture)
    {
        throw new NotImplementedException();
    }

    public string Get(string key, CultureInfo culture, params object[] arguments)
    {
        throw new NotImplementedException();
    }

    public string this[string key] => throw new NotImplementedException();

    public string this[string key, params object[] arguments] => throw new NotImplementedException();
}
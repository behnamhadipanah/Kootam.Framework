using Dapper;
using Kootam.Translator.Database.Models;
using Kootam.Translator.Database.Options;
using Microsoft.Extensions.Logging;

namespace Kootam.Translator.Database.Database;

internal static class TranslatorDatabaseInitializer
{
    public static void EnsureTable(
        IDbConnectionFactory connectionFactory,
        TranslatorOptions options,
        ILogger logger)
    {
        if (!options.AutoCreateSqlTable)
            return;

        try
        {
            using var connection = connectionFactory.CreateConnection();
            connection.Execute(TranslatorMigrationScripts.GetCreateTableScript(
                options.SchemaName,
                options.TableName));

            logger.LogInformation("Translator table ensured.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Create translator table failed");
            throw;
        }
    }

    public static void SeedDefaults(
        IDbConnectionFactory connectionFactory,
        TranslatorOptions options,
        ILogger logger,
        IReadOnlyDictionary<(string Key, string Culture), LocalizationRecord>? existingRecords = null)
    {
        var missing = options.DefaultTranslations
            .Where(item => existingRecords is null ||
                           !existingRecords.ContainsKey((item.Key, item.Culture)))
            .ToList();

        if (missing.Count == 0)
            return;

        var insertCommand =
            $"INSERT INTO [{options.SchemaName}].[{options.TableName}]([Key],[Value],[Culture]) " +
            $"SELECT @Key, @Value, @Culture " +
            $"WHERE NOT EXISTS (" +
            $"SELECT 1 FROM [{options.SchemaName}].[{options.TableName}] " +
            $"WHERE [Key] = @Key AND [Culture] = @Culture);";

        try
        {
            using var connection = connectionFactory.CreateConnection();

            foreach (var item in missing)
                connection.Execute(insertCommand, item);

            logger.LogInformation("Seeded default translations when missing");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Seed default translations failed");
            throw;
        }
    }
}

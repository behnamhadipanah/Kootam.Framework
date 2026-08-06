namespace Kootam.Translator.Database.Database;

public static class TranslatorMigrationScripts
{
    public static string GetCreateTableScript(string schemaName = "dbo", string tableName = "Translations")
        => $@"
IF NOT EXISTS (
    SELECT * FROM INFORMATION_SCHEMA.TABLES
    WHERE TABLE_SCHEMA = '{schemaName}'
      AND TABLE_NAME = '{tableName}'
)
BEGIN
    CREATE TABLE [{schemaName}].[{tableName}](
        Id BIGINT IDENTITY(1,1) PRIMARY KEY,
        BusinessId UNIQUEIDENTIFIER NOT NULL UNIQUE DEFAULT NEWID(),
        [Key] NVARCHAR(255) NOT NULL,
        [Value] NVARCHAR(500) NOT NULL,
        [Culture] NVARCHAR(5) NULL
    )
END";
}

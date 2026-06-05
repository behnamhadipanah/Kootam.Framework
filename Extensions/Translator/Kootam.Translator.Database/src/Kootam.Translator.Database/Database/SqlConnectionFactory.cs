using Microsoft.Data.SqlClient;
using System.Data;

namespace Kootam.Translator.Database.Database;

public class SqlConnectionFactory(string connectionString) : IDbConnectionFactory
{
    public IDbConnection CreateConnection() => new SqlConnection(connectionString);
}
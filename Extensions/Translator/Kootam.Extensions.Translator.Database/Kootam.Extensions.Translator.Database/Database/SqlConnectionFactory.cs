using Microsoft.Data.SqlClient;
using System.Data;

namespace Kootam.Extensions.Translator.Database.Database;

public class SqlConnectionFactory(string connectionString) : IDbConnectionFactory
{
    public IDbConnection CreateConnection() => new SqlConnection(connectionString);
}
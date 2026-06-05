using System.Data;

namespace Kootam.Translator.Database.Database;

public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();

}

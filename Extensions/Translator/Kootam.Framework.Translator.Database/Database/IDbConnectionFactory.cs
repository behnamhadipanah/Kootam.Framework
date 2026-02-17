using System.Data;

namespace Kootam.Framework.Translator.Database.Database;

public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();

}

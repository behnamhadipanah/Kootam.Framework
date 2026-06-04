using System.Data;

namespace Kootam.Extensions.Translator.Database.Database;

public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();

}

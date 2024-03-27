using System.Data;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;
// using MySql.Data.MySqlClient;
namespace todolist.Helpers;

public class DataContext
{
    private DbSetting _dbSetting;

    public DataContext(IOptions<DbSetting> dbSetting)
    {
        _dbSetting = dbSetting.Value;
    }

    public IDbConnection CreateConnection()
    {
        var connectionString = $"Server={_dbSetting.Server};Database={_dbSetting.Database};Uid={_dbSetting.Uid};Pwd={_dbSetting.Pwd};";
        return new MySqlConnection(connectionString);

    }
}
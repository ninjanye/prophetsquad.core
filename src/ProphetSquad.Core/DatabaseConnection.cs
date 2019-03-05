using System.Data;
using Dapper;

namespace ProphetSquad.Core
{
    public interface IDatabaseConnection
    {
        int Execute(string sql, object param = null);
    }

    public class DatabaseConnection : IDatabaseConnection
    {
        private IDbConnection _connection;

        public DatabaseConnection(IDbConnection connection)
        {
            _connection = connection;
        }
        
        public int Execute(string sql, object param = null)
        {
            return _connection.Execute(sql, param);
        }
    }
}
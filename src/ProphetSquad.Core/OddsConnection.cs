using System.Data;
using Dapper;

namespace ProphetSquad.Core
{
    public interface IOddsConnection
    {
        int Execute(string sql, object param = null);
    }

    public class OddsConnection : IOddsConnection
    {
        private IDbConnection _connection;

        public OddsConnection(IDbConnection connection)
        {
            _connection = connection;
        }
        
        public int Execute(string sql, object param = null)
        {
            return _connection.Execute(sql, param);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;

namespace ProphetSquad.Core
{
    public interface IDatabaseConnection
    {
        int Execute(string sql, object param = null);
        Task<IEnumerable<T>> Query<T>(string sql, object param = null);
        Task<IEnumerable<TReturn>> Query<TFirst, TSecond, TThird, TFourth, TReturn>(string sql, Func<TFirst, TSecond, TThird, TFourth, TReturn> map, object param = null, string splitOn = null);
        Task<T> QuerySingle<T>(string sql, object param = null);

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

        public async Task<IEnumerable<T>> Query<T>(string sql, object param = null)
        {
            return await _connection.QueryAsync<T>(sql, param);
        }

        public async Task<IEnumerable<TReturn>> Query<TFirst, TSecond, TThird, TFourth, TReturn>(string sql, Func<TFirst, TSecond, TThird, TFourth, TReturn> map, object param = null, string splitOn = null)
        {
            return await _connection.QueryAsync(sql, map, param: param, splitOn: splitOn);
        }


        public async Task<T> QuerySingle<T>(string sql, object param = null)
        {
            return await _connection.QuerySingleOrDefaultAsync<T>(sql, param);
        }
    }
}
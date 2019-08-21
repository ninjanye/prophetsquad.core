using System.Threading.Tasks;
using ProphetSquad.Core.Data.Models;

namespace ProphetSquad.Core
{
    public class RegionDatabase : IRegionDatabase
    {
        private readonly IDatabaseConnection _connection;
        private const string mergeSql = @"
BEGIN TRAN;
    WITH data as (SELECT @Name as Name)
    MERGE Regions r
    USING data d on d.Name = r.Name
    WHEN MATCHED 
        THEN UPDATE
            SET r.Code = @Code
    WHEN NOT MATCHED BY TARGET
        THEN INSERT (Code,Name,ModelState,Created)
             VALUES (@Code,@Name,0,GETDATE());
COMMIT TRAN;";

        public RegionDatabase(IDatabaseConnection connection)
        {
            _connection = connection;
        }

        public async Task<Region> GetBySourceId(int id)
        {
            const string selectStatement = "SELECT * FROM Regions WHERE Code = @id";
            return await _connection.QuerySingle<Region>(selectStatement, new { id });
        }

        public async Task<Region> Retrieve(string name)
        {
            const string selectStatement = "SELECT * FROM Regions WHERE Name = @name";
            return await _connection.QuerySingle<Region>(selectStatement, new { name });
        }

        public void Save(Region region)
        {
            _connection.Execute(mergeSql, region);
        }
    }
}
using ProphetSquad.Core.Data.Models;
using System.Threading.Tasks;

namespace ProphetSquad.Core.Databases
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
        THEN UPDATE SET
            r.Code = @Code,
            r.SeoUrl = @SeoUrl
    WHEN NOT MATCHED BY TARGET
        THEN INSERT (Code,Name,SeoUrl,ModelState,Created)
             VALUES (@Code,@Name,@SeoUrl,0,GETDATE());
COMMIT TRAN;";

        public RegionDatabase(IDatabaseConnection connection)
        {
            _connection = connection;
        }

        public async Task<Region> RetrieveBySourceId(int id)
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
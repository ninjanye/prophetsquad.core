using ProphetSquad.Core.Data.Models;
using ProphetSquad.Core.Providers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProphetSquad.Core.Databases
{
    public class CompetitionDatabase : IStore<Competition>, IProvider<Competition>
    {
        private readonly IDatabaseConnection _connection;

        public CompetitionDatabase(IDatabaseConnection connection)
        {
            _connection = connection;
        }

        public async Task<Competition> RetrieveBySourceId(int id)
        {
            const string selectStatement = "SELECT * FROM Competitions WHERE OpenFootyId = @id";
            return await _connection.QuerySingle<Competition>(selectStatement, new { id });
        }

        const string mergeSql = @"
BEGIN TRAN;
    WITH data as (SELECT @OpenFootyId as OpenFootyId)
    MERGE Competitions c
    USING data d on d.OpenFootyId = c.OpenFootyId
    WHEN MATCHED 
        THEN UPDATE SET 
            c.Name = @Name,
            c.SeoUrl = @SeoUrl
            c.RegionId = @RegionId            
    WHEN NOT MATCHED BY TARGET
        THEN INSERT (OpenFootyId, Name, SeoUrl, RegionId, Created, BookieId, ModelState)
             VALUES (@OpenFootyId, @Name, @SeoUrl, @RegionId, GETDATE(), 0, 0);
COMMIT TRAN;";

        public void Save(Competition competition)
        {
            _connection.Execute(mergeSql, competition);
        }

        public async Task<IEnumerable<Competition>> RetrieveAll()
        {
            const string selectStatement = "SELECT * FROM Competitions c WHERE c.ModelState = 0";
            return await _connection.Query<Competition>(selectStatement);
        }
    }
}
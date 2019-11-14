using ProphetSquad.Core.Data.Models;
using System.Threading.Tasks;

namespace ProphetSquad.Core.Databases
{
    public class TeamDatabase : IDatabase<Team>
    {
        private readonly IDatabaseConnection _connection;
        private const string mergeSql = @"
BEGIN TRAN;
    WITH data as (SELECT @OpenFootyId as OpenFootyId)
    MERGE Teams t
    USING data d on d.OpenFootyId = t.OpenFootyId
    WHEN MATCHED 
        THEN UPDATE SET 
            t.Name = @Name,
            t.BookieName = @BookieName,
            t.Badge = @Badge,
            t.SeoUrl = @SeoUrl
    WHEN NOT MATCHED BY TARGET
        THEN INSERT (OpenFootyId,Name,SeoUrl,BookieName,Badge,ModelState,Created)
             VALUES (@OpenFootyId,@Name,@SeoUrl,@BookieName,@Badge,0,GETDATE());
COMMIT TRAN;";

        public TeamDatabase(IDatabaseConnection connection)
        {
            _connection = connection;
        }

        public async Task<Team> GetBySourceId(int id)
        {
            const string selectStatement = "SELECT * FROM Teams WHERE OpenFootyId = @id";
            return await _connection.QuerySingle<Team>(selectStatement, new { id });
        }

        public void Save(Team team)
        {
            _connection.Execute(mergeSql, team);
        }
    }
}
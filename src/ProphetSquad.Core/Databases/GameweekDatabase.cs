using System;
using System.Threading.Tasks;
using ProphetSquad.Core.Data.Models;

namespace ProphetSquad.Core.Databases
{
    public class GameweekDatabase : IGameweekDatabase
    {
        private readonly IDatabaseConnection _connection;

        public GameweekDatabase(IDatabaseConnection connection)
        {
            _connection = connection;
        }

        public async Task<Gameweek> Retrieve(DateTime date)
        {
            const string selectStatement = "SELECT * FROM Gameweeks WHERE Start <= @date AND [End] >= @date";
            var gameweek = await _connection.QuerySingle<Gameweek>(selectStatement, new { date });
            if (gameweek is null)
            {
                var season = await GetSeason(date);
                if (season is null)
                {
                    CreateSeason(date);
                }

                CreateGameweek(date);
                gameweek = await _connection.QuerySingle<Gameweek>(selectStatement, new { date });
            }

            return gameweek;
        }

        private const string mergeSql = @"
BEGIN TRAN;
    WITH data as (SELECT @Start as Start, @End as [End])
    MERGE Gameweeks gw
    USING data d on d.Start = gw.Start 
        AND d.[End] = gw.[End]
    WHEN NOT MATCHED BY TARGET
        THEN INSERT (Start, [End], Processed, StatsProcessed, MoneyLeagueProcessed, ModelState, Created)
             VALUES (@Start, @End, 0, 0, 0, 0, GETDATE());
COMMIT TRAN;";

        private void CreateGameweek(DateTime date)
        {
            var start = date.Date;
            var end = start.AddDays(1).Subtract(new TimeSpan(TimeSpan.TicksPerSecond));
            _connection.Execute(mergeSql, new { start, end });
        }


        private async Task<Season> GetSeason(DateTime date)
        {
            const string selectStatement = "SELECT * FROM Seasons WHERE Start <= @date AND [End] >= @date";
            return await _connection.QuerySingle<Season>(selectStatement, new { date });
        }

        private void CreateSeason(DateTime date)
        {
            var start = new DateTime(date.Year, 8, 1);
            var end = start.AddYears(1);
            if (date.Month < 8)
            {
                start = start.AddYears(-1);
                end = end.AddYears(-1);
            }

            const string insertStatement = "INSERT INTO Seasons (Start, [End], ModelState, Created) VALUES(@start, @end, 0, GETDATE())";
            _connection.Execute(insertStatement, new { start, end });
        }
    }
}
using ProphetSquad.Core.Data.Models;
using ProphetSquad.Core.Databases;
using ProphetSquad.Core.Models.Betfair.Response;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace ProphetSquad.Core.Tests
{
    public class OddsDatabaseTests : IDatabaseConnection
    {
        private const string expectedSql = @"
BEGIN TRAN;
    WITH data as (SELECT @Id as Id)
    MERGE MatchOdds mo
    USING data d on d.Id = mo.Id
    WHEN MATCHED 
        THEN UPDATE
            SET mo.HomeTeamOdds = @HomeTeamOdds,
                mo.HomeTeamOddsDecimal = @HomeTeamOddsDecimal, 
                mo.AwayTeamOdds = @AwayTeamOdds, 
                mo.AwayTeamOddsDecimal = @AwayTeamOddsDecimal,
                mo.LastUpdate = GETDATE()
    WHEN NOT MATCHED BY TARGET
        THEN INSERT (Id,CompetitionId,CompetitionName,[Date],
                    HomeTeamId,HomeTeamName,HomeTeamOdds,
                    HomeTeamOddsDecimal,AwayTeamId,AwayTeamName,
                    AwayTeamOdds,AwayTeamOddsDecimal,LastUpdate,Processed)
             VALUES (@Id,@CompetitionId,@CompetitionName,@Date,
                    @HomeTeamId,@HomeTeamName,@HomeTeamOdds,
                    @HomeTeamOddsDecimal,@AwayTeamId,@AwayTeamName,
                    @AwayTeamOdds,@AwayTeamOddsDecimal,@LastUpdate,@Processed);
COMMIT TRAN;";

        private string _sql;
        private object _odds;

        [Fact]
        public void SavingOdds()
        {
            var database = new OddsDatabase(this);
            var competition = new Models.Betfair.Response.Competition { Id = "compId", Name = "compName" };
            var homeTeam = new Models.Betfair.Response.Team { SelectionId = "home", Name = "homeTeamName", Metadata = new Metadata { Id = "123456" }, Odds = 3.5m };
            var awayTeam = new Models.Betfair.Response.Team { SelectionId = "away", Name = "awayTeamName", Metadata = new Metadata { Id = "987654" }, Odds = 12m };
            var source = new Market { Id = "id", Competition = competition, StartTime = DateTime.UtcNow, Teams = new[] { homeTeam, awayTeam } };
            var odds = MatchOdds.From(source);

            database.Save(odds);

            Assert.Equal(expectedSql, _sql);
            Assert.Same(odds, _odds);
        }

        int IDatabaseConnection.Execute(string sql, object param)
        {
            _sql = sql;
            _odds = param;
            return 0;
        }

        Task<IEnumerable<T>> IDatabaseConnection.Query<T>(string sql, object param)
        {
            throw new NotImplementedException();
        }

        Task<IEnumerable<TReturn>> IDatabaseConnection.Query<TFirst, TSecond, TThird, TFourth, TReturn>(string sql, Func<TFirst, TSecond, TThird, TFourth, TReturn> map, object param, string splitOn)
        {
            throw new NotImplementedException();
        }

        Task<T> IDatabaseConnection.QuerySingle<T>(string sql, object param)
        {
            throw new NotImplementedException();
        }
    }
}
using System;
using ProphetSquad.Core.Data.Models;
using ProphetSquad.Core.Models.Betfair.Response;
using Xunit;

namespace ProphetSquad.Core.Tests
{
    public class OddsDatabaseTests : IOddsConnection
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
            var competition = new Competition{ Id = "compId", Name = "compName" };
            var homeTeam = new Team{SelectionId = "home", Name = "homeTeamName", Metadata = new Metadata { Id = "123456"}, Odds = 3.5m};
            var awayTeam = new Team{SelectionId = "away", Name = "awayTeamName", Metadata = new Metadata { Id = "987654"}, Odds = 12m};
            var source = new Market { Id = "id", Competition = competition, StartTime = DateTime.UtcNow, Teams = new[]{homeTeam, awayTeam} };
            var odds = MatchOdds.From(source);

            database.Save(odds);

            Assert.Equal(expectedSql, _sql);
            Assert.Same(odds, _odds);
        }

        int IOddsConnection.Execute(string sql, object param)
        {
            _sql = sql;
            _odds = param;
            return 0;
        }
    }
}
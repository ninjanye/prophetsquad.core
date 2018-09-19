using ProphetSquad.Core;
using ProphetSquad.Core.Data.Models;
using ProphetSquad.Core.Matcher;
using Xunit;

namespace ProphetSquad.Matcher.Tests
{
    public class FixtureDatabaseTests : IDatabaseConnection
    {
        private bool _executeCalled;
        private string _sqlReceived;
        private object _fixtureReceived;
        private const string expectedSql = @"
BEGIN TRAN;
    WITH data as (SELECT @Id as Id)
    MERGE Matches m
    USING data d on d.Id = m.Id
    WHEN MATCHED 
        THEN UPDATE
            SET m.MatchOddsId = @MatchOddsId
    WHEN NOT MATCHED BY TARGET
        THEN INSERT (OpenFootyId,CompetitionId,GameweekId,Date,
                    HomeTeamId,HomeTeamScore,AwayTeamId,AwayTeamScore,
                    WinnerId,IsResult,MatchOddsId,Processed,ModelState,Created)
             VALUES (@OpenFootyId,@CompetitionId,@GameweekId,@Date,
                    @HomeTeamId,@HomeTeamScore,@AwayTeamId,@AwayTeamScore,
                    @WinnerId,@IsResult,@MatchOddsId,@Processed,@ModelState,GETDATE());
COMMIT TRAN;";        

        [Fact]
        public void SaveToConnection()
        {
            var sut = new FixtureDatabase(this);
            sut.Save(new Fixture());
            Assert.True(_executeCalled);            
        }

        [Fact]
        public void SaveExecutesCorrectScript()
        {
            var sut = new FixtureDatabase(this);
            sut.Save(new Fixture());
            Assert.Equal(expectedSql, _sqlReceived);            

        }

        [Fact]
        public void SAveExecutesWithCorrectObject()
        {
            var sut = new FixtureDatabase(this);
            var fixture = new Fixture();
            sut.Save(fixture);
            Assert.Same(fixture, _fixtureReceived);
        }

        int IDatabaseConnection.Execute(string sql, object param)
        {
            _executeCalled = true;
            _sqlReceived = sql;
            _fixtureReceived = param;
            return 0;
        }
    }
}
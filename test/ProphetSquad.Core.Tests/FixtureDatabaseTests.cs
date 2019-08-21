using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using ProphetSquad.Core;
using ProphetSquad.Core.Data.Models;
using Xunit;
using Fixture = ProphetSquad.Core.Data.Models.Fixture;

namespace ProphetSquad.Matcher.Tests
{
    public class FixtureDatabaseTests : IDatabaseConnection
    {
        private bool _executeCalled;
        private string _sqlReceived;
        private object _fixtureReceived;
        private AutoFixture.Fixture _autoFixture;
        private const string expectedSql = @"
BEGIN TRAN;
    WITH data as (SELECT @OpenFootyId as OpenFootyId)
    MERGE Matches m
    USING data d on d.OpenFootyId = m.OpenFootyId
    WHEN MATCHED 
        THEN UPDATE
            SET m.MatchOddsId = COALESCE(@MatchOddsId, m.MatchOddsId)
    WHEN NOT MATCHED BY TARGET
        THEN INSERT (OpenFootyId,CompetitionId,GameweekId,Date,
                    HomeTeamId,HomeTeamScore,AwayTeamId,AwayTeamScore,
                    WinnerId,IsResult,MatchOddsId,Processed,ModelState,Created)
             VALUES (@OpenFootyId,@CompetitionId,@GameweekId,@Date,
                    @HomeTeamId,@HomeTeamScore,@AwayTeamId,@AwayTeamScore,
                    @WinnerId,@IsResult,@MatchOddsId,@Processed,@ModelState,GETDATE());
COMMIT TRAN;";

        public FixtureDatabaseTests()
        {
            _autoFixture = new AutoFixture.Fixture();
        }

        [Fact]
        public void SaveToConnection()
        {
            var sut = new FixtureDatabase(this);
            sut.Save(_autoFixture.Create<Fixture>());
            Assert.True(_executeCalled);            
        }

        [Fact]
        public void SaveExecutesCorrectScript()
        {
            var sut = new FixtureDatabase(this);
            sut.Save(_autoFixture.Create<Fixture>());
            Assert.Equal(expectedSql, _sqlReceived);            

        }

        [Fact]
        public void SaveExecutesWithCorrectObject()
        {
            var sut = new FixtureDatabase(this);
            var fixture = _autoFixture.Create<Fixture>();
            sut.Save(fixture);
            Assert.Same(fixture, _fixtureReceived);
        }

        int IDatabaseConnection.Execute(string sql, object param)
        {
            _executeCalled = true;

            if (_fixtureReceived == null)
            {
                //Only set on first execution
                _sqlReceived = sql;
                _fixtureReceived = param;
            }

            return 0;
        }

        async Task<IEnumerable<T>> IDatabaseConnection.Query<T>(string sql, object param)
        {
            return await Task.FromResult(new T[0]);
        }

        Task<T> IDatabaseConnection.QuerySingle<T>(string sql, object param = null)
        {
            throw new System.NotImplementedException();
        }

        Task<IEnumerable<TReturn>> IDatabaseConnection.Query<TFirst, TSecond, TThird, TFourth, TReturn>(string sql, Func<TFirst, TSecond, TThird, TFourth, TReturn> map, object param, string splitOn)
        {
            throw new NotImplementedException();
        }
    }
}
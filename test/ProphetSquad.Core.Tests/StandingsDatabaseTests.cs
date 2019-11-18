using AutoFixture;
using ProphetSquad.Core.Data.Models;
using ProphetSquad.Core.Databases;
using ProphetSquad.Core.Providers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace ProphetSquad.Core.Tests
{
    public class StandingsDatabaseTests : IDatabaseConnection, IProvider<Team>, IProvider<Competition>
    {
        private const string _expectedSql = @"
BEGIN TRAN;
    WITH data as (SELECT @CompetitionId as CompetitionId, @TeamId as TeamId)
    MERGE CompetitionPositions cp
    USING data d on d.CompetitionId = cp.CompetitionId
                AND d.TeamId = cp.TeamId
    WHEN MATCHED 
        THEN UPDATE SET 
            cp.Played = @Played,
            cp.Wins = @Wins,
            cp.Draws = @Draws,
            cp.Losses = @Losses,
            cp.GoalsFor = @GoalsFor,
            cp.GoalsAgainst = @GoalsAgainst,
            cp.Points = @Points,
            cp.Form = @Form
    WHEN NOT MATCHED BY TARGET
        THEN INSERT (CompetitionId,TeamId,Played,Wins,Draws,Losses,GoalsFor,GoalsAgainst,Points,Form)
             VALUES (@CompetitionId,@TeamId,@Played,@Wins,@Draws,@Losses,@GoalsFor,@GoalsAgainst,@Points,@Form);
COMMIT TRAN;";
        private readonly Standing _standing;
        private string _actualSql;
        private Standing _actualStanding;
        private Team _team;
        private bool _teamRetrieved;
        private Competition _competition;
        private bool _competitionRetrieved;

        public StandingsDatabaseTests()
        {
            var fixture = new AutoFixture.Fixture();
            _team = fixture.Create<Team>();
            _competition = fixture.Create<Competition>();
            _standing = fixture.Create<Standing>();
            var standingsDb = new StandingsDatabase(this, this, this);
            standingsDb.Save(_standing);
        }

        private bool _executeCalled;

        [Fact]
        public void SavingOddsCallsExecute()
        {
            Assert.True(_executeCalled);
        }

        [Fact]
        public void SavingOddsExecutesCorrectScript()
        {
            Assert.Equal(_expectedSql, _actualSql);
        }

        [Fact]
        public void SavingOddsHasSuppliedTeamId()
        {
            Assert.True(_teamRetrieved);
            Assert.Equal(_team.Id, _actualStanding.TeamId);
        }

        [Fact]
        public void SavingOddsHasSuppliedCompetitionId()
        {
            Assert.True(_competitionRetrieved);
            Assert.Equal(_competition.Id, _actualStanding.CompetitionId);
        }

        int IDatabaseConnection.Execute(string sql, object param)
        {
            _executeCalled = true;
            _actualSql = sql;
            _actualStanding = param as Standing;
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


        async Task<Team> IProvider<Team>.RetrieveBySourceId(int id)
        {
            _teamRetrieved = true;
            return await Task.FromResult(_team);
        }

        async Task<Competition> IProvider<Competition>.RetrieveBySourceId(int id)
        {
            _competitionRetrieved = true;
            return await Task.FromResult(_competition);
        }

        public Task<IEnumerable<Team>> RetrieveAll()
        {
            throw new NotImplementedException();
        }

        Task<IEnumerable<Competition>> IProvider<Competition>.RetrieveAll()
        {
            throw new NotImplementedException();
        }
    }
}
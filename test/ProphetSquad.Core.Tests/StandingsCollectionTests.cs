using AutoFixture;
using ProphetSquad.Core.Data.Models;
using ProphetSquad.Core.Databases;
using ProphetSquad.Core.Providers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ProphetSquad.Core.Tests
{
    public class StandingsCollectionTests : IProvider<Standing>, IStore<Standing>, IProvider<Competition>, IProvider<Team>
    {
        private readonly AutoFixture.Fixture _autoFixture;
        private readonly IEnumerable<Standing> _standings;
        private readonly StandingsCollection _result;
        private int _teamDbRetrieveCalled = 0;
        private int _dbSaveCalled = 0;
        private int _compDbRetrieveCalled = 0;
        private ICollection<Competition> _competitions = new List<Competition>();
        private ICollection<Team> _teams = new List<Team>();
        private ICollection<Standing> _standingsSaved = new List<Standing>();

        public StandingsCollectionTests()
        {
            _autoFixture = new AutoFixture.Fixture();
            _standings = _autoFixture.CreateMany<Standing>();
            _result = StandingsCollection.RetrieveFrom(this).Result;
        }

        [Fact]
        public void CollectionReturnsCorrectStandings()
        {
            Assert.Equal(_standings, _result);
        }

        [Fact]
        public async Task SaveToCallsDatabaseForEachStandingAsync()
        {
            await _result.SaveTo(this, this, this);

            Assert.Equal(_standings.Count(), _dbSaveCalled);
        }

        [Fact]
        public async Task SaveToGetsCompetitionIdForEachStandingAsync()
        {
            await _result.SaveTo(this, this, this);
            Assert.Equal(_standings.Count(), _compDbRetrieveCalled);
        }

        [Fact]
        public async Task EachStandingSavedHasCompetitionId()
        {
            await _result.SaveTo(this, this, this);
            var competitionIds = _competitions.Select(c => c.Id);
            var savedCompetitionIds = _standingsSaved.Select(s => s.CompetitionId);
            Assert.Equal(competitionIds, savedCompetitionIds);
        }

        [Fact]
        public async Task SaveToGetsTeamIdForEachStandingAsync()
        {
            await _result.SaveTo(this, this, this);
            Assert.Equal(_standings.Count(), _teamDbRetrieveCalled);
        }

        [Fact]
        public async Task EachStandingSavedHasTeamId()
        {
            await _result.SaveTo(this, this, this);
            var teamIds = _teams.Select(t => t.Id);
            var savedTeamIds = _standingsSaved.Select(s => s.TeamId);
            Assert.Equal(teamIds, savedTeamIds);
        }

        Task<Standing> IProvider<Standing>.RetrieveBySourceId(int id)
        {
            throw new System.NotImplementedException();
        }

        async Task<Competition> IProvider<Competition>.RetrieveBySourceId(int id)
        {
            _compDbRetrieveCalled++;
            var comp = _autoFixture.Create<Competition>();
            _competitions.Add(comp);
            return await Task.FromResult(comp);
        }

        Task<IEnumerable<Competition>> IProvider<Competition>.RetrieveAll()
        {
            throw new System.NotImplementedException();
        }


        async Task<IEnumerable<Standing>> IProvider<Standing>.RetrieveAll() => await Task.FromResult(_standings);

        void IStore<Standing>.Save(Standing standing)
        {
            _dbSaveCalled++;
            _standingsSaved.Add(standing);
        }

        async Task<Team> IProvider<Team>.RetrieveBySourceId(int id)
        {
            _teamDbRetrieveCalled++;
            var team = _autoFixture.Create<Team>();
            _teams.Add(team);
            return await Task.FromResult(team);
        }

        Task<IEnumerable<Team>> IProvider<Team>.RetrieveAll()
        {
            throw new System.NotImplementedException();
        }
    }
}
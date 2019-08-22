using ProphetSquad.Core.Data.Models;
using AutoFixture;
using Xunit;
using Fixture = ProphetSquad.Core.Data.Models.Fixture;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using ProphetSquad.Core.Data.Models.FootballDataApi;
using ProphetSquad.Core.Databases;
using ProphetSquad.Core.Mappers;

namespace ProphetSquad.Core.Tests
{
    public class FixtureMapperTests : IDatabase<Data.Models.Competition>, IDatabase<Data.Models.Team>, IGameweekDatabase
    {
        private readonly Data.Models.Competition _competition;
        private readonly Gameweek _gameweek;
        private readonly Data.Models.Team _homeTeam;
        private readonly Data.Models.Team _awayTeam;
        private readonly MatchResponse _source;
        private readonly IEnumerable<Fixture> _result;
        private bool _gameweekRetrieved;
        private bool _competitionRetrieved;

        public FixtureMapperTests()
        {
            var autoFixture = new AutoFixture.Fixture();

            _competition = autoFixture.Create<Data.Models.Competition>();
            _gameweek = autoFixture.Create<Gameweek>();
            _homeTeam = autoFixture.Create<Data.Models.Team>();
            _awayTeam = autoFixture.Create<Data.Models.Team>();
            _source = autoFixture.Create<MatchResponse>();

            var mapper = new FixtureMapper(this, this, this);
            _result = mapper.MapAsync(_source).Result;
        }

        [Fact]
        public void ReturnsFixture()
        {
            Assert.IsAssignableFrom<IEnumerable<Fixture>>(_result);
        }

        [Fact]
        public void CorrectNumberOfFixturesReturned()
        {
            Assert.Equal(_source.Matches.Length, _result.Count());
        }

        [Fact]
        public void MapSourcePropertiesToFixture()
        {
            var sourceMatch = _source.Matches[0];
            var fixture = _result.First();

            Assert.Equal(sourceMatch.Id, fixture.OpenFootyId);
            Assert.Equal(sourceMatch.Date, fixture.Date);
            Assert.Equal(_competition.Id, fixture.CompetitionId);
            Assert.Equal(_competition, fixture.Competition);

            Assert.Equal(_homeTeam, fixture.HomeTeam);
            Assert.Equal(_homeTeam.Id, fixture.HomeTeamId);
            Assert.Equal(sourceMatch.Score.FullTime.HomeTeam, fixture.HomeTeamScore);
            Assert.Equal(_awayTeam, fixture.AwayTeam);
            Assert.Equal(_awayTeam.Id, fixture.AwayTeamId);
            Assert.Equal(sourceMatch.Score.FullTime.AwayTeam, fixture.AwayTeamScore);
            Assert.Equal(sourceMatch.Status == "FINISHED", fixture.IsResult);
            Assert.Equal(_gameweek.Id, fixture.GameweekId);
        }

        [Fact]
        public void CalculatesWinnerCorrectly()
        {
            int? expected = _source.Matches[0].Score.Winner == "HOME_TEAM" ? _homeTeam.Id :
                            _source.Matches[0].Score.Winner == "AWAY_TEAM" ? _awayTeam.Id : (int?)null;

            Assert.Equal(expected, _result.First().WinnerId);
        }

        [Fact]
        public void GetsCompetitionFromDatabase()
        {
            Assert.True(_competitionRetrieved);
        }

        [Fact]
        public void RetrievesGameweek()
        {
            Assert.True(_gameweekRetrieved);
        }

        public void Save(Data.Models.Competition fixture)
        {
            throw new System.NotImplementedException();
        }

        public Task<Data.Models.Competition> GetBySourceId(int id)
        {
            _competitionRetrieved = true;
            return Task.FromResult(_competition);
        }

        void IDatabase<Data.Models.Team>.Save(Data.Models.Team fixture)
        {
            throw new System.NotImplementedException();
        }

        int _teamCount;
        Task<Data.Models.Team> IDatabase<Data.Models.Team>.GetBySourceId(int id)
        {
            return Task.FromResult(_teamCount++ == 0 ? _homeTeam : _awayTeam);
        }

        public async Task<Gameweek> Retrieve(DateTime date)
        {
            _gameweekRetrieved = true;
            return await Task.FromResult(_gameweek);
        }
    }
}
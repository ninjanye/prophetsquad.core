﻿using AutoFixture;
using ProphetSquad.Core.Data.Models;
using ProphetSquad.Core.Data.Models.FootballDataApi;
using ProphetSquad.Core.Databases;
using ProphetSquad.Core.Mappers;
using ProphetSquad.Core.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Fixture = ProphetSquad.Core.Data.Models.Fixture;

namespace ProphetSquad.Core.Tests
{
    public class FixtureMapperTests : IProvider<Data.Models.Competition>, IProvider<Data.Models.Team>, IGameweekDatabase
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

        public void Save(Data.Models.Competition fixture) => throw new NotImplementedException();

        int _teamCount;
        Task<Data.Models.Team> IProvider<Data.Models.Team>.RetrieveBySourceId(int id)
        {
            return Task.FromResult(_teamCount++ == 0 ? _homeTeam : _awayTeam);
        }

        public async Task<Gameweek> Retrieve(DateTime date)
        {
            _gameweekRetrieved = true;
            return await Task.FromResult(_gameweek);
        }

        Task<IEnumerable<Data.Models.Team>> IProvider<Data.Models.Team>.RetrieveAll()
        {
            throw new NotImplementedException();
        }

        Task<IEnumerable<Data.Models.Competition>> IProvider<Data.Models.Competition>.RetrieveAll()
        {
            throw new NotImplementedException();
        }

        Task<Data.Models.Competition> IProvider<Data.Models.Competition>.RetrieveBySourceId(int id)
        {
            _competitionRetrieved = true;
            return Task.FromResult(_competition);
        }
    }
}
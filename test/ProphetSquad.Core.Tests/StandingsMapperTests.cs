using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using ProphetSquad.Core.Data.Models.FootballDataApi;
using ProphetSquad.Core.Mappers;
using Xunit;

namespace ProphetSquad.Core.Tests
{
    public class StandingsMapperTests
    {
        private readonly StandingResponse _source;
        private readonly IEnumerable<Data.Models.Standing> _result;

        public StandingsMapperTests()
        {
            var autoFixture = new AutoFixture.Fixture();
            autoFixture.Customize<Standing>(x => x.With(s => s.Stage, "REGULAR_SEASON"));
            _source = autoFixture.Create<StandingResponse>();

            var rnd = new Random();
            var index = rnd.Next(_source.Standings.Length);
            _source.Standings[index].Type = "TOTAL";

            var mapper = new StandingsMapper();
            _result = mapper.MapAsync(_source).Result;
        }

        [Fact]
        public void ReturnsStanding()
        {
            Assert.IsAssignableFrom<IEnumerable<Data.Models.Standing>>(_result);
        }

        [Fact]
        public void CorrectNumberOfStandingsReturned()
        {
            Assert.Equal(_source.Standings.Length, _result.Count());
        }

        [Fact]
        public void MapSourcePropertiesToStanding()
        {
            var source = _source.Standings.FirstOrDefault(s => s.Stage == "REGULAR_SEASON" && s.Type == "TOTAL");
            var standing = _result.First();

            var table = source.Table[0];
            Assert.Equal(table.PlayedGames, standing.Played);
            Assert.Equal(table.Won, standing.Wins);
            Assert.Equal(table.Lost, standing.Losses);
            Assert.Equal(table.Draw, standing.Draws);
            Assert.Equal(table.GoalsFor, standing.GoalsFor);
            Assert.Equal(table.GoalsAgainst, standing.GoalsAgainst);
            Assert.Equal(table.Points, standing.Points);
            Assert.Equal(table.Team.Id, standing.SourceTeamId);
        }

    }
}
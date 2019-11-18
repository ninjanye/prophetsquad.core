using ProphetSquad.Core;
using ProphetSquad.Core.Data.Models;
using ProphetSquad.Core.Databases;
using ProphetSquad.Core.Models.Betfair.Response;
using ProphetSquad.Core.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ProphetSquad.Matcher.Tests
{
    public class WhenOddsAreSuccessfullyMatched_UpdateFixture : IFixtureProvider, IProvider<MatchOdds>, IFixturesDatabase
    {
        private readonly MatchOdds _matchOdds;
        private IEnumerable<MatchOdds> _oddsReturned = Enumerable.Empty<MatchOdds>();
        private readonly Fixture _fixture;
        private IEnumerable<Fixture> _fixturesReturned = Enumerable.Empty<Fixture>();
        private List<Fixture> _savedFixtures = new List<Fixture>();
        private bool _fixturesSaved;
        private readonly OddsMatcher _oddsMatcher;

        public WhenOddsAreSuccessfullyMatched_UpdateFixture()
        {
            int homeTeamId = 111;
            int awayTeamId = homeTeamId + 1;
            var competition = new Core.Models.Betfair.Response.Competition { Id = "123456789", Name = "competition" };
            var homeTeam = new Core.Models.Betfair.Response.Team { Name = "homeTeam", Odds = 1.5m, SelectionId = "homeSelection", Metadata = new Metadata { Id = homeTeamId.ToString() } };
            var awayTeam = new Core.Models.Betfair.Response.Team { Name = "awayTeam", Odds = 5m, SelectionId = "awaySelection", Metadata = new Metadata { Id = awayTeamId.ToString() } };
            DateTime matchStart = DateTime.UtcNow.AddDays(2);
            var market = new Market
            {
                Id = "marketId",
                Competition = competition,
                Name = "market",
                StartTime = matchStart,
                Teams = new[] { homeTeam, awayTeam }
            };

            _matchOdds = MatchOdds.From(market);
            _oddsReturned = new[] { _matchOdds };
            _fixture = new Fixture
            {
                Date = matchStart,
                CompetitionId = 1,
                Competition = new Core.Data.Models.Competition { BookieId = 123456789 },
                //HomeTeamId = homeTeamId,
                HomeTeam = new Core.Data.Models.Team { BookieName = "homeTeam" },
                //AwayTeamId = awayTeamId,
                AwayTeam = new Core.Data.Models.Team { BookieName = "homeTeam" }
            };
            _fixturesReturned = new[] { _fixture };

            _oddsMatcher = new OddsMatcher(this, this, this);
            _oddsMatcher.Synchronise().Wait();
        }

        [Fact]
        public void MatchOddsIdIsSet()
        {
            Assert.Equal(_matchOdds.Id, _fixture.MatchOddsId);
        }

        [Fact]
        public void MatchOddsCompetitionBookieIdSet()
        {
            Assert.Equal(Convert.ToInt64(_matchOdds.CompetitionId), _fixture.Competition.BookieId);
        }

        [Fact]
        public void MatchOddsHomeTeamBookieNameSet()
        {
            Assert.Equal(_matchOdds.HomeTeamName, _fixture.HomeTeam.BookieName);
        }

        [Fact]
        public void MatchOddsAwayTeamBookieNameSet()
        {
            Assert.Equal(_matchOdds.AwayTeamName, _fixture.AwayTeam.BookieName);
        }

        [Fact]
        public void FixtureUpdatesAreSaved()
        {
            Assert.True(_fixturesSaved);
            Assert.Equal(_fixturesReturned, _savedFixtures);
        }

        async Task<IEnumerable<Fixture>> IFixtureProvider.Retrieve(DateTime from, DateTime to)
        {
            return await Task.FromResult(_fixturesReturned);
        }

        Task<IEnumerable<MatchOdds>> IProvider<MatchOdds>.RetrieveAll()
        {
            return Task.FromResult(_oddsReturned);
        }

        Task<MatchOdds> IProvider<MatchOdds>.RetrieveBySourceId(int id)
        {
            throw new NotImplementedException();
        }

        Task<IEnumerable<Fixture>> IFixturesDatabase.Retrieve(DateTime from, DateTime to)
        {
            throw new NotImplementedException();
        }

        void IStore<Fixture>.Save(Fixture fixture)
        {
            _fixturesSaved = true;
            _savedFixtures.Add(fixture);
        }
    }
}
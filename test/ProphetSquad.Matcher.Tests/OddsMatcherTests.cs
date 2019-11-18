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
    public class OddsMatcherTests : IFixtureProvider, IProvider<MatchOdds>, IFixturesDatabase
    {
        private bool _fixturesRetrieved;
        private DateTime _fixturesRetrievedFrom;
        private DateTime _fixturesRetrievedTo;
        private bool _oddsRetrieved;
        private IEnumerable<MatchOdds> _oddsReturned = Enumerable.Empty<MatchOdds>();
        private IEnumerable<Fixture> _fixturesReturned = Enumerable.Empty<Fixture>();
        private readonly OddsMatcher _oddsMatcher;
        private Random _randomiser = new Random();

        public OddsMatcherTests()
        {
            _oddsMatcher = new OddsMatcher(this, this, this);
        }

        [Fact]
        public async Task RetrievesFixtures()
        {
            await _oddsMatcher.Synchronise();
            Assert.True(_fixturesRetrieved);
        }

        [Fact]
        public async Task RetrievesFixturesForNextSevenDays()
        {
            var startTime = DateTime.UtcNow;
            await _oddsMatcher.Synchronise();
            Assert.True(_fixturesRetrievedFrom > startTime);
            Assert.True(_fixturesRetrievedFrom < DateTime.UtcNow);
            Assert.Equal(DateTime.Today.AddDays(7), _fixturesRetrievedTo);
        }

        [Fact]
        public async Task RetrievesOdds()
        {
            await _oddsMatcher.Synchronise();
            Assert.True(_oddsRetrieved);
        }

        [Fact]
        public async Task DoNotUpdateFixturesInThePast()
        {
            var matchedFixture = CreateFixture();
            matchedFixture.Date = DateTime.UtcNow.AddHours(-2);
            _fixturesReturned = new[] { matchedFixture };

            var odds = CreateOddsFrom(matchedFixture);
            _oddsReturned = new[] { odds };

            await _oddsMatcher.Synchronise();

            Assert.Null(matchedFixture.MatchOddsId);
        }

        [Fact]
        public async Task DoNotUpdateFixturesWithOddsMatched()
        {
            const string matchedOddsId = "matched";
            var matchedFixture = CreateFixture();
            matchedFixture.MatchOddsId = matchedOddsId;
            _fixturesReturned = new[] { matchedFixture };
            var odds = CreateOddsFrom(matchedFixture);
            _oddsReturned = new[] { odds };

            await _oddsMatcher.Synchronise();

            Assert.Equal(matchedOddsId, matchedFixture.MatchOddsId);
        }

        [Fact]
        public async Task DoesNotUpdateFixtureIfMatchTimeIsNotWithinAnHour()
        {
            var matchedFixture = CreateFixture();
            _fixturesReturned = new[] { matchedFixture };
            var odds = CreateOddsFrom(matchedFixture, date: matchedFixture.Date.AddHours(2));
            _oddsReturned = new[] { odds };

            await _oddsMatcher.Synchronise();

            Assert.Null(matchedFixture.MatchOddsId);
        }

        [Fact]
        public async Task DoesNotUpdateFixtureIfMatchTimeIsBefore()
        {
            var matchedFixture = CreateFixture();
            _fixturesReturned = new[] { matchedFixture };
            var odds = CreateOddsFrom(matchedFixture, date: matchedFixture.Date.AddHours(-2));
            _oddsReturned = new[] { odds };

            await _oddsMatcher.Synchronise();

            Assert.Null(matchedFixture.MatchOddsId);
        }


        [Fact]
        public async Task DoNotUpdateFixtureIfCompetitionIdsAndTeamNamesDiffer()
        {
            var matchedFixture = CreateFixture();
            _fixturesReturned = new[] { matchedFixture };
            var odds = CreateOddsFrom(matchedFixture, competitionId: matchedFixture.CompetitionId + 1, competitionName: $"NEW {matchedFixture.Competition.Name}", homeTeamName: $"NEW {matchedFixture.HomeTeam.Name}");
            _oddsReturned = new[] { odds };

            await _oddsMatcher.Synchronise();

            Assert.True(string.IsNullOrEmpty(matchedFixture.MatchOddsId));
        }

        [Fact]
        public async Task UpdateFixtureIfCompetitionIdsDifferButNamesMatch()
        {
            var matchedFixture = CreateFixture();
            _fixturesReturned = new[] { matchedFixture };
            var odds = CreateOddsFrom(matchedFixture, competitionId: matchedFixture.CompetitionId + 1);
            _oddsReturned = new[] { odds };

            await _oddsMatcher.Synchronise();

            Assert.Equal(odds.Id, matchedFixture.MatchOddsId);
        }

        [Fact]
        public async Task UpdateFixtureIfAwayTeamIsNotSetButOthersMatch()
        {
            var partialMatchedFixture = CreateFixture();
            partialMatchedFixture.HomeTeamId = 0;
            _fixturesReturned = new[] { partialMatchedFixture };
            var odds = CreateOddsFrom(partialMatchedFixture, awayTeamId: partialMatchedFixture.AwayTeamId + 1);
            _oddsReturned = new[] { odds };

            await _oddsMatcher.Synchronise();

            Assert.Equal(odds.Id, partialMatchedFixture.MatchOddsId);
        }

        [Fact]
        public async Task UpdateFixtureIdsDifferButTeamNamesMatch()
        {
            var matchedFixture = CreateFixture();
            _fixturesReturned = new[] { matchedFixture };
            var odds = CreateOddsFrom(matchedFixture, homeTeamId: matchedFixture.HomeTeamId + 1, awayTeamId: matchedFixture.AwayTeamId + 1);
            _oddsReturned = new[] { odds };

            await _oddsMatcher.Synchronise();

            Assert.Equal(odds.Id, matchedFixture.MatchOddsId);
        }

        private Fixture CreateFixture()
        {
            return new Fixture
            {
                Date = DateTime.UtcNow.AddDays(_randomiser.Next(1, 100)),
                CompetitionId = _randomiser.Next(1, 1000),
                Competition = new Core.Data.Models.Competition { Name = $"CompetitionName{_randomiser.Next(1, 100)}" },
                HomeTeamId = _randomiser.Next(1, 1000),
                HomeTeam = new Core.Data.Models.Team { BookieName = $"HomeTeam{_randomiser.Next(1, 100)}" },
                AwayTeamId = _randomiser.Next(1, 1000),
                AwayTeam = new Core.Data.Models.Team { BookieName = $"AwayTeam{_randomiser.Next(1, 100)}" }
            };
        }

        private MatchOdds CreateOddsFrom(
            Fixture fixture,
            int? competitionId = null,
            string competitionName = null,
            int? homeTeamId = null,
            string homeTeamName = null,
            int? awayTeamId = null,
            string awayTeamName = null,
            DateTime? date = null)
        {
            int resolvedCompetitionId = competitionId ?? fixture.CompetitionId;
            string resolvedCompetitionName = competitionName ?? fixture.Competition.Name;
            int resolvedHomeTeamId = homeTeamId ?? fixture.HomeTeamId;
            string resolvedHomeTeamName = homeTeamName ?? fixture.HomeTeam.BookieName;
            int resolvedAwayTeamId = awayTeamId ?? fixture.AwayTeamId;
            string resolvedAwayTeamName = awayTeamName ?? fixture.AwayTeam.BookieName;
            var resolvedDate = date ?? fixture.Date;

            var homeTeam = new Core.Models.Betfair.Response.Team { Name = resolvedHomeTeamName, Odds = 1.5m, SelectionId = "homeSelection", Metadata = new Metadata { Id = resolvedHomeTeamId.ToString() } };
            var awayTeam = new Core.Models.Betfair.Response.Team { Name = resolvedAwayTeamName, Odds = 5m, SelectionId = "awaySelection", Metadata = new Metadata { Id = resolvedAwayTeamId.ToString() } };
            var competition = new Core.Models.Betfair.Response.Competition { Id = resolvedCompetitionId.ToString(), Name = resolvedCompetitionName };
            return MatchOdds.From(new Market { Id = $"marketId{_randomiser.Next(1, 1000)}", StartTime = resolvedDate, Competition = competition, Teams = new[] { homeTeam, awayTeam } });
        }

        async Task<IEnumerable<Fixture>> IFixtureProvider.Retrieve(DateTime from, DateTime to)
        {
            _fixturesRetrieved = true;
            _fixturesRetrievedFrom = from;
            _fixturesRetrievedTo = to;
            return await Task.FromResult(_fixturesReturned);
        }

        Task<IEnumerable<MatchOdds>> IProvider<MatchOdds>.RetrieveAll()
        {
            _oddsRetrieved = true;
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
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProphetSquad.Core;
using ProphetSquad.Core.Data.Models;
using ProphetSquad.Core.Matcher;
using ProphetSquad.Core.Models.Betfair.Response;
using Xunit;

namespace ProphetSquad.Matcher.Tests
{
    public class OddsMatcherTests : IFixtureProvider, IOddsProvider
    {
        private bool _fixturesRetrieved;
        private bool _oddsRetrieved;
        private IEnumerable<MatchOdds> _oddsReturned = Enumerable.Empty<MatchOdds>();
        private IEnumerable<Fixture> _fixturesReturned = Enumerable.Empty<Fixture>();
        private readonly OddsMatcher _oddsMatcher;

        public OddsMatcherTests()
        {
            _oddsMatcher = new OddsMatcher(this, this);
        }

        [Fact]
        public void RetrievesFixtures()
        {
            _oddsMatcher.Synchronise();
            Assert.True(_fixturesRetrieved);
        }

        [Fact]
        public void RetrievesOdds()
        {
            _oddsMatcher.Synchronise();
            Assert.True(_oddsRetrieved);
        }

        [Fact]
        public void DoNotUpdateFixturesInThePast()
        {
            var pastFixture = new Fixture{ Date = DateTime.UtcNow.AddDays(-1)};
            _fixturesReturned = new[]{ pastFixture }; 
            var odds = MatchOdds.From(new Market{ StartTime = pastFixture.Date});
            _oddsReturned = new[]{ odds }; 
            _oddsMatcher.Synchronise();
             Assert.Null(pastFixture.MatchOddsId);     
        }

        [Fact]
        public void DoNotUpdateFixturesWithOddsMatched()
        {
            const string matchedOddsId = "matched";
            var matchedFixture = new Fixture { MatchOddsId = matchedOddsId };
            _fixturesReturned = new[]{ matchedFixture }; 
            var odds = MatchOdds.From(new Market{ StartTime = matchedFixture.Date});
            _oddsReturned = new[]{ odds }; 
            _oddsMatcher.Synchronise();
            Assert.Equal(matchedOddsId, matchedFixture.MatchOddsId);
        }

        [Fact]
        public void DoesNotUpdateFixtureIfMatchTimeIsNotWithinAnHour()
        {
            DateTime fixtureDate = DateTime.UtcNow.AddDays(1);
            var fixture = new Fixture { Date = fixtureDate};
            _fixturesReturned = new[]{ fixture };
            var odds = MatchOdds.From(new Market{ StartTime = fixtureDate.AddHours(2)});
            _oddsReturned = new[]{ odds }; 

            _oddsMatcher.Synchronise();

            Assert.Null(fixture.MatchOddsId);
        }

        IEnumerable<Fixture> IFixtureProvider.Retrieve()
        {
            _fixturesRetrieved = true;
            return _fixturesReturned;
        }

        Task<IEnumerable<MatchOdds>> IOddsProvider.RetrieveAsync()
        {
            _oddsRetrieved = true;
            return Task.FromResult(_oddsReturned);
        }
    }
}
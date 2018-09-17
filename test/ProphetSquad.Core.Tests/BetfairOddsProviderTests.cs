using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProphetSquad.Core.Data.Models;
using ProphetSquad.Core.Models.Betfair.Response;
using Xunit;

namespace ProphetSquad.Core.Tests
{
    public class BetfairOddsProviderTests: IBetfairClient
    {
        private bool _betfairClientCalled;
        private IOddsProvider oddsProvider;
        private IEnumerable<MatchOdds> odds;
        private Market _firstOdd;
        private IEnumerable<Market> _betfairOdds;

        public BetfairOddsProviderTests()
        {
            oddsProvider = new BetfairOddsProvider(this);
            _betfairOdds = BuildBetfairOdds();

            odds = oddsProvider.RetrieveAsync().Result;
        }

        private Market[] BuildBetfairOdds()
        {
            _firstOdd = new Market{ Id = "test", Competition = new Competition { Id = "CompetitionId"} };
            var another = new Market{ Id = "another", Competition = new Competition { Id = "AnotherCompetitionId"} };
            return new[] { _firstOdd, another };
        }

        [Fact]
        public void RetrieveReturnsMatchOdds()
        {
            Assert.IsAssignableFrom<IEnumerable<MatchOdds>>(odds);
        }

        [Fact]
        public void RetrieveAskBetfairClientForOdds()
        {
            Assert.True(_betfairClientCalled);
        }

        [Fact]
        public void RetrieveReturnsOddsFromBetfairClient()
        {
            Assert.Equal(_betfairOdds.Count(), odds.Count());
            for (int i = 0; i < odds.Count(); i++)
            {
                Assert.Equal(_betfairOdds.ElementAt(i).Id, odds.ElementAt(i).Id);
            }
        }

        [Fact]
        public void MatchOddsMapping()
        {
            var result = odds.First();
            var expected = _betfairOdds.First();
            Assert.Equal(expected.Id, result.Id);
            Assert.Equal(expected.Competition.Id, result.CompetitionId);
        }

        Task<IEnumerable<Market>> IBetfairClient.GetOdds()
        {
            _betfairClientCalled = true;
            return Task.FromResult(_betfairOdds);
        }
    }
}
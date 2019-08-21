using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using ProphetSquad.Core.Data.Models;
using ProphetSquad.Core.Models.Betfair.Response;
using Xunit;

namespace ProphetSquad.Core.Tests
{
    public class BetfairOddsProviderTests : IHttpClient, IAuthenticator
    {
        private readonly AutoFixture.Fixture _autoFixture;
        private IOddsProvider oddsProvider;
        private IEnumerable<MatchOdds> odds;
        private Market _firstOdd;
        private List<Market> _betfairOdds = new List<Market>();
        private readonly List<string> _requestedEndpoints = new List<string>();

        public BetfairOddsProviderTests()
        {
            _autoFixture = new AutoFixture.Fixture();
            oddsProvider = new BetfairOddsProvider(this, this);

            odds = oddsProvider.RetrieveAsync().Result;
        }

        [Fact]
        public void RetrieveReturnsMatchOdds()
        {
            Assert.IsAssignableFrom<IEnumerable<MatchOdds>>(odds);
        }

        [Fact]
        public void RetrieveAskBetfairClientForOdds()
        {
            Assert.Contains(_requestedEndpoints, x => x.EndsWith("listMarketBook/"));
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

        async Task<string> IAuthenticator.GetAuthTokenAsync()
        {
            return await Task.FromResult("token");
        }

        Task<T> IHttpClient.Get<T>(string authToken, string endpoint)
        {
            return Task.FromResult(new T());
        }

        Task<T> IHttpClient.Post<T>(string endpoint, HttpContent httpContent)
        {
            _requestedEndpoints.Add(endpoint);

            var result = _autoFixture.Create<T>();
            if (result.GetType().IsAssignableFrom(_betfairOdds.GetType()))
            {
                _betfairOdds.AddRange(result as List<Market>);
            }

            return Task.FromResult(result);
        }
    }
}
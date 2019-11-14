using ProphetSquad.Core.Data.Models;
using ProphetSquad.Core.Models.Betfair.Response;
using ProphetSquad.Core.Providers.Betfair;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace ProphetSquad.Core.Tests.BetfairClientTests
{
    public class WhenRetrievingOddsFails : IHttpClient, IAuthenticator, IThrottler
    {
        private BetfairOddsProvider _client;
        private readonly IEnumerable<MatchOdds> _result;
        private readonly ICollection<string> _requestedEndpoints;
        private List<Country> _countries;

        public WhenRetrievingOddsFails()
        {
            _requestedEndpoints = new List<string>();
            _client = new BetfairOddsProvider(this, this, this);
            var country1 = new Country { CountryCode = "TEST1" };
            var country2 = new Country { CountryCode = "TEST2" };
            _countries = new List<Country> { country1, country2 };

            _result = _client.RetrieveAsync().Result;
        }

        [Fact]
        public void DoesNotReturnNull()
        {
            Assert.Empty(_result);
        }

        Task<T> IHttpClient.Get<T>(string authToken, string endpoint)
        {
            return Task.FromResult(new T());
        }

        Task<T> IHttpClient.Post<T>(string endpoint, HttpContent httpContent)
        {
            _requestedEndpoints.Add(endpoint);
            return Task.FromResult(null as T);
        }

        Task<string> IAuthenticator.GetAuthTokenAsync() => Task.FromResult("TOKEN");

        void IThrottler.Wait()
        {
        }
    }
}
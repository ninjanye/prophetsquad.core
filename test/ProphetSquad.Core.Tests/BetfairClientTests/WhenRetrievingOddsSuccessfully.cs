using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ProphetSquad.Core.Models.Betfair.Response;
using Xunit;

namespace ProphetSquad.Core.Tests.BetfairClientTests
{
    public class WhenRetrievingOddsSuccessfully : IHttpClient, IAuthenticator
    {
        private BetfairClient _client;
        private readonly IEnumerable<Market> _result;
        private readonly ICollection<string> _requestedEndpoints;
        private List<Country> _countries;
        private List<Market> _expected = new List<Market>();
        const string baseUrl = "https://api.betfair.com/exchange/betting/rest/v1.0/";
        const string listCountries = baseUrl + "listCountries/";
        const string listMarketCatalogue = baseUrl + "listMarketCatalogue/";
        const string listMarketBook = baseUrl + "listMarketBook/";

        public WhenRetrievingOddsSuccessfully()
        {
            _requestedEndpoints = new List<string>();
            _client = new BetfairClient(this, this);            
            var country1 = new Country{ CountryCode = "TEST1" };
            var country2 = new Country{ CountryCode = "TEST2" };
            _countries = new List<Country> { country1, country2 };

            _result = _client.GetOdds().Result;
        }

        [Fact]
        public void DoesNotReturnNull()
        {
            Assert.NotNull(_result);
        }

        [Fact]
        public void ReturnsResultFromHttpClient()
        {
            Assert.Equal(_expected, _result);
        }
        
        [Theory]
        [InlineData(listCountries, 1)]
        public void RequestsUrlsFor(string expectedUrl, int count)
        {
            Assert.Equal(count, _requestedEndpoints.Count(url => url == expectedUrl));
        }

        [Theory]
        [InlineData(listMarketBook, 2)]
        [InlineData(listMarketCatalogue, 2)]
        public void RequestsUrlsAtLeast(string expectedUrl, int count)
        {
            Assert.True(_requestedEndpoints.Count(url => url == expectedUrl) >= count);
        }


        Task<T> IHttpClient.Get<T>(string authToken, string endpoint)
        {
            return Task.FromResult(new T());
        }

        Task<T> IHttpClient.Post<T>(string endpoint, HttpContent httpContent)
        {
            _requestedEndpoints.Add(endpoint);

            switch (endpoint)
            {
                case listCountries:
                    return Task.FromResult(_countries as T);
                case listMarketCatalogue:
                    var marketOne = new Market{ Id =$"{DateTime.UtcNow.Second}.{DateTime.UtcNow.Millisecond}"};
                    var marketTwo = new Market{ Id =$"{DateTime.UtcNow.Second}.{DateTime.UtcNow.Millisecond}"};
                    var markets = new List<Market>{ marketOne, marketTwo };
                    _expected.Add(marketOne);
                    _expected.Add(marketTwo);
                    return Task.FromResult(markets as T);
                default:
                    return Task.FromResult(new T());
            }
        }

        Task<string> IAuthenticator.GetAuthTokenAsync()
        {
            return Task.FromResult("TOKEN");
        }
    }
}
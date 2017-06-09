using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace ProphetSquad.Core.Tests.BetfairClientTests
{
    public class WhenRetrievingOddsSuccessfully_Auth : IHttpClient, IAuthenticator
    {
        private bool _authTokenRequested;
        private const string _authToken = "authToken";
        private bool _authTokenUsed;
        private string _authTokenSent;
        private BetfairClient _client;

        public WhenRetrievingOddsSuccessfully_Auth()
        {
            _client = new BetfairClient(this, this);
            var result = _client.GetOdds().Result;            
        }

        [Fact]
        public void AuthTokenRequested()
        {
            Assert.True(_authTokenRequested);
        }

        [Fact]
        public void AuthTokenSentToHttpClient()
        {
            Assert.True(_authTokenUsed);
            Assert.Equal(_authToken, _authTokenSent);
        }

        Task<T> IHttpClient.Get<T>(string authToken, string endpoint)
        {
            return Task.FromResult(new T());
        }

        Task<T> IHttpClient.Post<T>(string endpoint, HttpContent httpContent)
        {
            const string authenticationHeader = "X-Authentication";
            if(httpContent.Headers.Contains(authenticationHeader)){
                _authTokenUsed = true;
                _authTokenSent = httpContent.Headers.GetValues(authenticationHeader).First();
            }

            return Task.FromResult(new T());
        }

        Task<string> IAuthenticator.GetAuthTokenAsync()
        {
            _authTokenRequested = true;
            return Task.FromResult(_authToken);
        }
    }
}
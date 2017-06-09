using System;
using System.Net.Http;
using System.Security.Authentication;
using System.Threading.Tasks;
using Xunit;

namespace ProphetSquad.Core.Tests.BetfairAuthenticatorTests
{
    public class WhenAuthenticatingFails : IHttpClient
    {
        private BetfairAuthenticator _authenticator;

        public WhenAuthenticatingFails()
        {
            _authenticator = new BetfairAuthenticator(this, string.Empty, string.Empty);
        }

        [Fact]
        public async Task AuthenticatorThrowsException()
        {
            await Assert.ThrowsAsync<AuthenticationException>(async () => await _authenticator.GetAuthTokenAsync());
        }
        
        Task<T> IHttpClient.Get<T>(string authToken, string endpoint)
        {
            throw new NotImplementedException();
        }

        Task<T> IHttpClient.Post<T>(string endpoint, HttpContent httpContent)
        {
            return Task.Factory.StartNew(() => Newtonsoft.Json.JsonConvert.DeserializeObject<T>("{ token: \"\", error: \"An error meesage\", status: \"FAIL\" }"));
        }
    }
}
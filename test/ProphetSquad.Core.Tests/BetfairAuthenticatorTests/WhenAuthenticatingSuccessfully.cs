using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;
using Xunit;

namespace ProphetSquad.Core.Tests.BetfairAuthenticatorTests
{
    public class WhenAuthenticatingSuccessfully : IHttpClient
    {
        private bool _httpPostCalled;
        private Type _requestedType;
        private string _expectedToken => "test";
        private string _requestedEndpoint;
        private string _requestedUsername;
        private string _requestedPassword;
        private BetfairAuthenticator _authenticator;
        private string _token;
        private int _requestCount;

        public WhenAuthenticatingSuccessfully()
        {
            _authenticator = new BetfairAuthenticator(this, "username", "password");
            _token = _authenticator.GetAuthTokenAsync().Result;
        }

        [Fact]
        public void AuthenticatorPostsDataViaHttpClient()
        {
            Assert.True(_httpPostCalled);
        }

        [Fact]
        public void AuthenticatorRequestsAuthenticationResult()
        {
            Assert.Equal("AuthenticationResult", _requestedType.Name);
        }

        [Fact]
        public void AuthenticatorReturnsTokenFromHttpClient()
        {
            Assert.Equal(_expectedToken, _token);
        }

        [Fact]
        public void PostsToAuthEndpoint(){
            var expectedEndpoint = "https://identitysso.betfair.com/api/login";
            Assert.Equal(expectedEndpoint, _requestedEndpoint);
        }

        [Fact]
        public void PostsCredentials(){
            Assert.NotEmpty(_requestedUsername);
            Assert.NotEmpty(_requestedPassword);
        }

        [Fact]
        public void TokenIsNotRequestedMoreThanOnce(){
            Assert.Equal(1, _requestCount);
            var token = _authenticator.GetAuthTokenAsync().Result;
            Assert.Equal(_expectedToken, token);
            Assert.Equal(1, _requestCount);
        }

        Task<T> IHttpClient.Get<T>(string authToken, string endpoint)
        {
            throw new NotImplementedException();
        }

        async Task<T> IHttpClient.Post<T>(string endpoint, HttpContent httpContent)
        {
            _requestCount++;
            _requestedType = typeof(T);        
            _requestedEndpoint = endpoint;
            _requestedUsername = await GetValue(httpContent, "username");
            _requestedPassword = await GetValue(httpContent, "password");
            _httpPostCalled = true;

            return await Task.Run(() => Newtonsoft.Json.JsonConvert.DeserializeObject<T>("{ token: \"" + _expectedToken + "\" }"));
        }

        private static async Task<string> GetValue(HttpContent httpContent, string key)
        {
            var content = await httpContent.ReadAsStringAsync();
            var data = QueryHelpers.ParseQuery(content);
            return data.ContainsKey(key) ? data[key].ToString() : string.Empty;
        }
    }
}
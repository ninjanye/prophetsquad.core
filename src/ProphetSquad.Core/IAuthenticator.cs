using System;
using System.Security.Authentication;
using System.Threading.Tasks;

namespace ProphetSquad.Core
{
    public interface IAuthenticator
    {
        Task<string> GetAuthTokenAsync();
    }

    public class BetfairAuthenticator : IAuthenticator
    {
        const string AuthEnpoint = "https://identitysso.betfair.com/api/login";
        private IHttpClient _httpClient;
        private string _cachedToken;
        private string _username;
        private string _password;

        public BetfairAuthenticator(IHttpClient httpClient, string username, string password)
        {
            _username = username;
            _password = password;
            _httpClient = httpClient;
        }

        public async Task<string> GetAuthTokenAsync()
        {
            if(!String.IsNullOrEmpty(_cachedToken))
            {
                return _cachedToken;
            }

            using (var authorisationRequest = BetfairRequest.Authorise(_username, _password)) 
            {
                var result = await authorisationRequest.Submit<AuthenticationResult>(_httpClient);
                if(string.IsNullOrEmpty(result.Token))
                {
                    Console.WriteLine($"[ERROR] Unable to authenticate");
                    throw new AuthenticationException("Unable to authenticate");
                }
                _cachedToken = result.Token;
                return _cachedToken;            
            }
        }

        private class AuthenticationResult
        {
            public string Token { get; set; }
        }
    }
}
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ProphetSquad.Core
{
    public interface IHttpRequest : IDisposable
    {
        Task<T> Submit<T>(IHttpClient httpClient) where T : class, new();
    }

    public class HttpRequest : IHttpRequest
    {
        private readonly string _endpoint;
        private string _authToken;
        protected readonly HttpContent _httpContent;
        private readonly HttpMethod _httpMethod;
        private IAuthenticator _authenticator;

        public HttpRequest(HttpRequestConfiguration config)
        {
            _endpoint = config.Endpoint;
            _httpContent = config.HttpContent;
            _httpMethod = config.HttpMethod;
            _authenticator = config.Authenticator;
        }

        public async Task<T> Submit<T>(IHttpClient httpClient) where T : class, new()
        {
            if(_authenticator != null)
            {
                _authToken = await _authenticator.GetAuthTokenAsync();
                _httpContent.Headers.Add("X-Authentication", _authToken);
            }

            switch (_httpMethod.Method)
            {
                case "POST":
                    return await httpClient.Post<T>(_endpoint, _httpContent);                
                default:
                    return await httpClient.Get<T>(_authToken, _endpoint);
            }
        }

        public void Dispose()
        {
            _httpContent.Dispose();
        }
    }
}
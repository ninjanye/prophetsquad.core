using System.Net.Http;

namespace ProphetSquad.Core
{
    public class HttpRequestConfiguration
    {
        public HttpRequestConfiguration(string endpoint, HttpContent httpContent, IAuthenticator authenticator = null)
        {
            Endpoint = endpoint;
            HttpMethod = HttpMethod.Post;
            HttpContent = httpContent;
            Authenticator = authenticator;
        }
        public string Endpoint { get; }
        public HttpContent HttpContent { get; }
        public HttpMethod HttpMethod { get; }
        public IAuthenticator Authenticator { get; }
    }
}
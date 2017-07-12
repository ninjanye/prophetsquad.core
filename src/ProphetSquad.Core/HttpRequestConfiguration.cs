using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using ProphetSquad.Core.Models.Betfair.Request;

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
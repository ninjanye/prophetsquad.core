using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using ProphetSquad.Core.Models.Betfair.Request;

namespace ProphetSquad.Core
{
    public interface IHttpRequest : IDisposable
    {
        Task<T> Submit<T>(IHttpClient httpClient) where T : class, new();
    }

    public class HttpRequest : IHttpRequest
    {
        private const string restApiBase = "https://api.betfair.com/exchange/betting/rest/v1.0/";
        private readonly string _endpoint;
        private string _authToken;
        protected readonly HttpContent _httpContent;
        private readonly HttpMethod _httpMethod;
        private IAuthenticator _authenticator;

        public static IHttpRequest Authorise(string username, string password)
        {
            const string AuthEnpoint = "https://identitysso.betfair.com/api/login";
            var authData = new Dictionary<string, string>{
                { "username", username },
                { "password", password }
            };

            var httpConfig = new HttpRequestConfiguration(AuthEnpoint, new FormUrlEncodedContent(authData)); 
            return new HttpRequest(httpConfig);
        }

        private const string CatalogueEndpoint = restApiBase + "listMarketCatalogue/";
        public static HttpRequest GetCatalogue(IAuthenticator authenticator, string countryCode)
        {
            var filter = new Filter();
            filter.MarketCountries = new HashSet<string>{ countryCode };
            filter.EventTypeIds = new HashSet<string>{ "1" };
            filter.MarketTypeCodes = new HashSet<string>{ "MATCH_ODDS" };
            var marketProjection = new HashSet<string>{ "COMPETITION", "RUNNER_METADATA", "MARKET_DESCRIPTION", "MARKET_START_TIME" };
            var data = new { filter = filter, marketProjection, maxResults = 200 };
            var jsonContent = new JsonContent(data);
            var httpConfig = new HttpRequestConfiguration(CatalogueEndpoint, jsonContent, authenticator);
            return new HttpRequest(httpConfig);
        }

        private const string CountriesEndpoint = restApiBase + "listCountries/";
        public static HttpRequest GetCountries(IAuthenticator authenticator)
        {
            var data = new { filter = new Filter()};
            var jsonContent = new JsonContent(data);
            var httpConfig = new HttpRequestConfiguration(CountriesEndpoint, jsonContent, authenticator);
            return new HttpRequest(httpConfig);
        }

        private const string OddsEndpoint = restApiBase + "listMarketBook/";
        public static HttpRequest GetOdds(IAuthenticator authenticator, List<string> marketIds)
        {
            var priceDepth = new { bestPricesDepth = 1 };
            var priceProjection = new {
                priceData = new string[] { "EX_BEST_OFFERS"}, 
                exBestOffersOverrides = priceDepth
            };

            var data = new { marketIds = marketIds, priceProjection = priceProjection};
            var jsonContent = new JsonContent(data);
            var httpConfig = new HttpRequestConfiguration(OddsEndpoint, jsonContent, authenticator);
            return new HttpRequest(httpConfig);
        }

        private HttpRequest(HttpRequestConfiguration config)
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

        private class HttpRequestConfiguration
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
}
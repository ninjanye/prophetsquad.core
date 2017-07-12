using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using ProphetSquad.Core.Models.Betfair.Request;

namespace ProphetSquad.Core
{
    public class BetfairRequest
    {
        private const string restApiBase = "https://api.betfair.com/exchange/betting/rest/v1.0/";

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
    }
}
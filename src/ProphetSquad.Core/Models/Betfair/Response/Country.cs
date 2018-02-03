using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProphetSquad.Core.Models.Betfair.Response
{
    internal class Country
    {
        private List<Market> _markets;
        public string CountryCode { get; set; }
        public int MarketCount { get; set; }

        public async Task<IEnumerable<Market>> SoccerOdds(IHttpClient httpClient, IAuthenticator authenticator)
        {
            if (_markets == null)
            {
                _markets = new List<Market>();

                for (int i = 0; i < 10; i++)
                {
                    using(var marketRequest = BetfairRequest.GetCatalogue(authenticator, CountryCode))
                    {
                        var markets = await marketRequest.Submit<List<Market>>(httpClient);
                        var marketIds = markets.Select(m => m.Id).ToList();
                        using(var oddsRequest = BetfairRequest.GetOdds(authenticator, marketIds))
                        {                        
                            var odds = await oddsRequest.Submit<List<MarketBook>>(httpClient);
                            Console.WriteLine($"Retrieving odds for {CountryCode}... COMPLETE");
                            foreach (var market in markets)
                            {
                                market.PopulateOdds(odds);
                            }
                        }
                        _markets.AddRange(markets);
                    }
                }
            }
            return _markets;
        }
    }
}
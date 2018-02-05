using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProphetSquad.Core.Models.Betfair.Request;

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

                var startTime = DateTime.Today;
                for (int i = 0; i < 5; i++)
                {
                    var timeRange = new TimeRange{From = startTime.AddDays(i), To = startTime.AddDays(i+1)};
                    using(var marketRequest = BetfairRequest.GetCatalogue(authenticator, CountryCode, timeRange))
                    {
                        var markets = await marketRequest.Submit<List<Market>>(httpClient);
                        if(markets == null || markets.Count == 0)
                        {
                            Console.WriteLine($"[ERROR] Unable to get odds for {CountryCode}");
                            break;
                        }

                        var marketIds = markets.Select(m => m.Id).ToList();
                        if(marketIds.Any())
                        {
                            using(var oddsRequest = BetfairRequest.GetOdds(authenticator, marketIds))
                            {                        
                                var odds = await oddsRequest.Submit<List<MarketBook>>(httpClient);
                                if(odds == null) 
                                {
                                    Console.WriteLine($"Retrieving odds for {CountryCode}... FAILED");
                                    break;
                                }
                                Console.WriteLine($"Retrieving odds for {CountryCode}... COMPLETE ({odds.Count} records)");
                                foreach (var market in markets)
                                {
                                    market.PopulateOdds(odds);
                                }
                            }
                            _markets.AddRange(markets);
                        }
                    }
                }
            }
            return _markets;
        }
    }
}
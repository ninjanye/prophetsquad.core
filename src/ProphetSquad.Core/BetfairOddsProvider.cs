using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProphetSquad.Core.Data.Models;
using ProphetSquad.Core.Models.Betfair.Response;

namespace ProphetSquad.Core
{
    public class BetfairOddsProvider : IOddsProvider
    {
        private IHttpClient _httpClient;
        private IAuthenticator _authenticator;

        public BetfairOddsProvider(IHttpClient httpClient, IAuthenticator authenticator)
        {
            _authenticator = authenticator;
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<MatchOdds>> RetrieveAsync()
        {
            var tasks = new List<Task<IEnumerable<Market>>>();
            using (var countryRequest = BetfairRequest.GetCountries(_authenticator))
            {
                var countries = await countryRequest.Submit<List<Country>>(_httpClient);
                if (countries == null)
                {
                    Console.WriteLine($"[ERROR] Unable to get countries");
                    return Enumerable.Empty<MatchOdds>();
                }

                Console.WriteLine($"Retrieving data for {countries.Count} countries...");
                int i = 0;
                foreach (var country in countries)
                {
                    Console.WriteLine($"Retrieving odds for {country.CountryCode} [{++i} of {countries.Count}]...");
                    var scopedCountry = country;
                    tasks.Add(scopedCountry.SoccerOdds(_httpClient, _authenticator));
                    await Task.Delay(1000);
                }
            }

            var odds = Task.WhenAll(tasks).Result.SelectMany(x => x).ToList();
            return odds.Select(MatchOdds.From);
        }
    }
}
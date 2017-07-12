using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProphetSquad.Core.Models.Betfair.Response;

namespace ProphetSquad.Core
{
    public interface IBetfairClient
    {
        Task<IEnumerable<Market>> GetOdds();
    }

    public class BetfairClient : IBetfairClient
    {
        private IHttpClient _httpClient;
        private IAuthenticator _authenticator;

        public BetfairClient(IHttpClient httpClient, IAuthenticator authenticator)
        {
            _authenticator = authenticator;
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<Market>> GetOdds()
        {
            var tasks = new List<Task<IEnumerable<Market>>>();
            using (var countryRequest = BetfairRequest.GetCountries(_authenticator))
            {
                var countries = await countryRequest.Submit<List<Country>>(_httpClient);
                Console.WriteLine($"Retrieving data for {countries.Count} countries...");
                int i = 0;
                foreach (var country in countries)
                {
                    Console.WriteLine($"Retrieving odds for {country.CountryCode} [{++i} of {countries.Count}]...");
                    var scopedCountry = country;
                    tasks.Add(scopedCountry.SoccerOdds(_httpClient, _authenticator));
                }
            }

            return Task.WhenAll(tasks).Result.SelectMany(x => x).ToList();
        }
    }
}
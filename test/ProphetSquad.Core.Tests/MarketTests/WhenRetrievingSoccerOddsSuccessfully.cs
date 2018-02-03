using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ProphetSquad.Core.Models.Betfair.Request;
using ProphetSquad.Core.Models.Betfair.Response;
using Xunit;

namespace ProphetSquad.Core.Tests.CountryTests
{
    public class WhenRetrievingSoccerOddsSuccessfully : IHttpClient, IAuthenticator
    {
        const string baseUrl = "https://api.betfair.com/exchange/betting/rest/v1.0/";
        const string listMarketCatalogue = baseUrl + "listMarketCatalogue/";
        const string listMarketBook = baseUrl + "listMarketBook/";
        private readonly List<string> requestedUrls = new List<string>();
        private List<RequestData> requestedFilters = new List<RequestData>();
        private readonly List<Market> markets;
        private readonly List<MarketBook> marketBook;
        private readonly Country country;
        private readonly IEnumerable<Market> result;

        public WhenRetrievingSoccerOddsSuccessfully()
        {
            markets = new List<Market>{ BuildMarket("1"), BuildMarket("2")};
            marketBook = new List<MarketBook> { BuildMarketBook("1"), BuildMarketBook("2")};

            country = new Country { CountryCode = "TST" };
            result = country.SoccerOdds(this, this).Result;
        }

        [Theory]
        [InlineData(listMarketCatalogue)]
        [InlineData(listMarketBook)]
        public void MakesExpectedRequests(string endpoint)
        {
            Assert.Contains(endpoint, requestedUrls);
        }

        [Fact]
        public void CountryCodeFilterSubmitted()
        {
            Assert.All(requestedFilters, rf => {
                Assert.NotNull(rf.Filter.MarketCountries);
                Assert.Contains(country.CountryCode, rf.Filter.MarketCountries);            
            });
        }

        [Fact]
        public void MatchOddsRequested()
        {
            Assert.All(requestedFilters, rf => {
                Assert.NotNull(rf.Filter.MarketTypeCodes);
                Assert.Contains("MATCH_ODDS", rf.Filter.MarketTypeCodes);            
            });
        }

        [Fact]
        public void DateFilterApplied()
        {
            Assert.All(requestedFilters, rf => {
                Assert.NotNull(rf.Filter.MarketStartTime);
                Assert.True(rf.Filter.MarketStartTime.From < DateTime.UtcNow, "MarketStartTime is not less than now");

                Console.WriteLine($"requestFilter start time: {rf.Filter.MarketStartTime.From.Ticks}-{rf.Filter.MarketStartTime.To.Ticks}");
                //TODO: Check each request passes different date filter
                foreach (var filter in requestedFilters)
                {
                    Assert.True((rf.Filter.MarketStartTime.From <= filter.Filter.MarketStartTime.From 
                                 && rf.Filter.MarketStartTime.To <= filter.Filter.MarketStartTime.To)
                              ||(rf.Filter.MarketStartTime.From >= filter.Filter.MarketStartTime.From 
                                 && rf.Filter.MarketStartTime.To >= filter.Filter.MarketStartTime.To)); 
                }
            });
        }        

        [Fact]
        public void MarketHasOddsPopulated()
        {
            Assert.NotEmpty(result);
            var originalMarket = markets.First();
            var marketResult = result.First(x => x.Id == originalMarket.Id);
            Assert.Same(originalMarket, marketResult);

            var expectedHomeOdds = marketBook.First(mb => mb.MarketId == originalMarket.Id).Teams.First().Odds;
            Assert.Equal(expectedHomeOdds, marketResult.Teams.First().Odds);

            var expectedAwayOdds = marketBook.First(mb => mb.MarketId == originalMarket.Id).Teams.Last().Odds;
            Assert.Equal(expectedHomeOdds, marketResult.Teams.Last().Odds);

        }
        
        [Theory]
        [InlineData(listMarketCatalogue, 10)]
        public void RequestsUrlsFor(string expectedUrl, int count)
        {
            Assert.Equal(count, requestedUrls.Count(url => url == expectedUrl));
        }


        Task<T> IHttpClient.Get<T>(string authToken, string endpoint)
        {
            throw new NotImplementedException();
        }

        async Task<T> IHttpClient.Post<T>(string endpoint, HttpContent httpContent)
        {
            requestedUrls.Add(endpoint);
            switch (endpoint)
            {
                case listMarketCatalogue:
                    var content = await httpContent.ReadAsStringAsync();
                    requestedFilters.Add(JsonConvert.DeserializeObject<RequestData>(content));
                    return await Task.FromResult(markets as T);
                case listMarketBook:
                    return await Task.FromResult(marketBook as T);
                default:
                    return await Task.FromResult(new T());
            }
        }

        Task<string> IAuthenticator.GetAuthTokenAsync()
        {
            return Task.FromResult("auth_token");
        }

        private static Market BuildMarket(string id)
        {
            string homeId = $"{id}.homeTeam";
            var homeTeam = BuildTeam(homeId);

            string awayId = $"{id}.awayTeam";
            var awayTeam = BuildTeam(awayId);

            return new Market
            {
                Id = id,
                Teams = new[] { homeTeam, awayTeam }
            };
        }

        private static Team BuildTeam(string id)
        {
            return new Team
            {
                SelectionId = id,
                Name = id,
                Metadata = new Metadata { Id = id }
            };
        }

        private static MarketBook BuildMarketBook(string id)
        {
            return new MarketBook {
                Status = "OK",
                MarketId = id,
                Teams = new List<TeamOdds>{ 
                    BuildTeamOdds($"{id}.homeTeam"),
                    BuildTeamOdds($"{id}.awayTeam")
                }
            };
        }

        private static TeamOdds BuildTeamOdds(string id)
        {
            var rnd = new Random();
            var price = rnd.Next(1, 100);
            var priceDetails = new PriceDetails{ Price = price / 10 };

            return new TeamOdds {
                SelectionId = id,
                Exchange = new Exchange {
                    AvailableToBack = new[]{ priceDetails }
                }
            };            
        }

        private class RequestData
        {
            public Filter Filter { get; set; }
        }

        private class Filter
        {
            public string[] EventTypeIds { get; set; }
            public List<string> MarketCountries { get; set; }
            public string[] MarketTypeCodes { get; set; }
            public TimeRange MarketStartTime { get; set; }
        } 
    }
}
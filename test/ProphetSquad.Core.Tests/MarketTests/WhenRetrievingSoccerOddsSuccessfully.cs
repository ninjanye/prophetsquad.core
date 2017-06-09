using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
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
        private readonly List<Market> markets;
        private readonly List<MarketBook> marketBook;

        public WhenRetrievingSoccerOddsSuccessfully()
        {
            markets = new List<Market>{ BuildMarket("1"), BuildMarket("2")};
            marketBook = new List<MarketBook> { BuildMarketBook("1"), BuildMarketBook("2")};
        }

        [Theory]
        [InlineData(listMarketCatalogue)]
        [InlineData(listMarketBook)]
        public async void MakesExpectedRequests(string endpoint)
        {
            var country = new Country { CountryCode = "TST" };
            var result = await country.SoccerOdds(this, this);
            Assert.Contains(endpoint, requestedUrls);
        }

        [Fact]
        public async void MarketHasOddsPopulated()
        {
            var country = new Country { CountryCode = "TST" };
            var result = await country.SoccerOdds(this, this);
            Assert.NotEmpty(result);
            var originalMarket = markets.First();
            var marketResult = result.First(x => x.Id == originalMarket.Id);
            Assert.Same(originalMarket, marketResult);

            var expectedHomeOdds = marketBook.First(mb => mb.MarketId == originalMarket.Id).Teams.First().Odds;
            Assert.Equal(expectedHomeOdds, marketResult.Teams.First().Odds);

            var expectedAwayOdds = marketBook.First(mb => mb.MarketId == originalMarket.Id).Teams.Last().Odds;
            Assert.Equal(expectedHomeOdds, marketResult.Teams.Last().Odds);

        }

        Task<T> IHttpClient.Get<T>(string authToken, string endpoint)
        {
            throw new NotImplementedException();
        }

        Task<T> IHttpClient.Post<T>(string endpoint, HttpContent httpContent)
        {
            requestedUrls.Add(endpoint);
            switch (endpoint)
            {
                case listMarketCatalogue:
                    return Task.FromResult(markets as T);
                case listMarketBook:
                    return Task.FromResult(marketBook as T);
                default:
                    return Task.FromResult(new T());
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
    }
}
using ProphetSquad.Core.Data.Models;
using ProphetSquad.Core.Databases;
using ProphetSquad.Core.Models.Betfair.Response;
using ProphetSquad.Core.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ProphetSquad.Core.Tests
{
    public class OddsCollectionTests : IProvider<MatchOdds>, IStore<MatchOdds>
    {
        private MatchOdds odds1 = MatchOdds.From(BuildMarket("test one"));
        private MatchOdds odds2 = MatchOdds.From(BuildMarket("test two"));
        private List<MatchOdds> retrievedOdds = new List<MatchOdds>();
        private List<MatchOdds> savedOdds = new List<MatchOdds>();

        public OddsCollectionTests()
        {
            retrievedOdds = new List<MatchOdds> { odds1, odds2 };
        }

        [Fact]
        public async Task RetrievingFromReturnsCorrectOdds()
        {
            var collection = await OddsCollection.RetrieveFrom(this);

            Assert.Equal(retrievedOdds.Count, collection.Count());
            Assert.Single(collection, x => x.Id == odds1.Id);
            Assert.Single(collection, x => x.Id == odds2.Id);
        }

        [Fact]
        public async Task SaveToSavesEachRecord()
        {
            var collection = await OddsCollection.RetrieveFrom(this);

            collection.SaveTo(this);

            Assert.Equal(retrievedOdds, savedOdds);
        }

        Task<IEnumerable<MatchOdds>> IProvider<MatchOdds>.RetrieveAll()
        {
            return Task.FromResult(retrievedOdds.AsEnumerable());
        }

        Task<MatchOdds> IProvider<MatchOdds>.RetrieveBySourceId(int id)
        {
            throw new NotImplementedException();
        }

        void IStore<MatchOdds>.Save(MatchOdds item) => savedOdds.Add(item);

        private static Market BuildMarket(string marketId)
        {
            var homeTeam = new Models.Betfair.Response.Team { Metadata = new Metadata { Id = "home" }, Name = "home", Odds = 3.8m };
            var awayTeam = new Models.Betfair.Response.Team { Metadata = new Metadata { Id = "away" }, Name = "away", Odds = 6.4m };
            var competition = new Models.Betfair.Response.Competition { Id = "comp", Name = "comp" };
            return new Market
            {
                Id = marketId,
                StartTime = DateTime.UtcNow,
                Competition = competition,
                Teams = new[] { homeTeam, awayTeam }
            };
        }
    }
}
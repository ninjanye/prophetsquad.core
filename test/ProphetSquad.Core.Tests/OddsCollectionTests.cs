using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProphetSquad.Core.Data.Models;
using ProphetSquad.Core.Models.Betfair.Response;
using Xunit;

namespace ProphetSquad.Core.Tests
{
    public class OddsCollectionTests : IOddsProvider, IOddsDatabase
    {
        private MatchOdds odds1 = MatchOdds.From(BuildMarket("test one"));
        private MatchOdds odds2 = MatchOdds.From(BuildMarket("test two"));
        private List<MatchOdds> retrievedOdds = new List<MatchOdds>();
        private List<MatchOdds> savedOdds = new List<MatchOdds>();

        public OddsCollectionTests()
        {
            retrievedOdds = new List<MatchOdds>{odds1, odds2};
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

        Task<IEnumerable<MatchOdds>> IOddsProvider.Retrieve()
        {
            return Task.FromResult(retrievedOdds.AsEnumerable());
        }

        void IOddsDatabase.Save(MatchOdds odds)
        {
            savedOdds.Add(odds);
        }

        private static Market BuildMarket(string marketId)
        {
            var homeTeam = new Team { Metadata = new Metadata {Id = "home" }, Name = "home", Odds = 3.8m };
            var awayTeam = new Team { Metadata = new Metadata {Id = "away" }, Name = "away", Odds = 6.4m };
            var competition = new Competition { Id = "comp", Name = "comp" };
            return new Market { 
                Id = marketId,
                StartTime = DateTime.UtcNow,
                Competition = competition,
                Teams = new[] {homeTeam, awayTeam}
            };
        }
    }
}
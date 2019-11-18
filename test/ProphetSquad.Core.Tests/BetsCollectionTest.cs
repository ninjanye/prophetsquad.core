using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using ProphetSquad.Core.Collections;
using ProphetSquad.Core.Data.Models;
using ProphetSquad.Core.Databases;
using ProphetSquad.Core.Providers;
using Xunit;

namespace ProphetSquad.Core.Tests
{
    public class BetsCollectionTest : IProvider<Bet>, IStore<Bet>
    {
        private readonly IEnumerable<Bet> _bets;
        private readonly List<Bet> _betsSaved = new List<Bet>();
        private readonly BetCollection _betCollection;

        public BetsCollectionTest()
        {
            var fixture = new AutoFixture.Fixture();
            _bets = fixture.CreateMany<Bet>();
            _betCollection = BetCollection.RetrieveFrom(this).Result;
        }

        [Fact]
        public void ReturnsCollectionOfBets()
        {
            Assert.IsAssignableFrom<IEnumerable<Bet>>(_betCollection);
        }

        [Fact]
        public void ReturnsCorrectNumberOfBets()
        {
            Assert.Equal(_bets, _betCollection);
        }

        [Fact]
        public void SavesEachBetSupplied()
        {
            _betCollection.SaveTo(this);
            Assert.Equal(_bets, _betsSaved);
        }

        async Task<IEnumerable<Bet>> IProvider<Bet>.RetrieveAll()
        {
            return await Task.FromResult(_bets);
        }

        Task<Bet> IProvider<Bet>.RetrieveBySourceId(int id)
        {
            throw new System.NotImplementedException();
        }

        void IStore<Bet>.Save(Bet item)
        {
            _betsSaved.Add(item);
        }
    }
}
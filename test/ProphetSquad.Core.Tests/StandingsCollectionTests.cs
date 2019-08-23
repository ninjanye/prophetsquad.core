using AutoFixture;
using ProphetSquad.Core.Data.Models;
using ProphetSquad.Core.Databases;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ProphetSquad.Core.Tests
{
    public class StandingsCollectionTests : IProvider<Standing>, IDatabase<Standing>
    {
        private readonly IEnumerable<Standing> _standings;
        private readonly StandingsCollection _result;
        private int _dbSaveCalled = 0;

        public StandingsCollectionTests()
        {
            var autoFixture = new AutoFixture.Fixture();
            _standings = autoFixture.CreateMany<Standing>();
            _result = StandingsCollection.RetrieveFrom(this).Result;
        }

        [Fact]
        public void CollectionReturnsCorrectStandings()
        {
            Assert.Equal(_standings, _result);
        }

        [Fact]
        public void SaveToCallsDatabaseForEachStanding()
        {
            _result.SaveTo(this);

            Assert.Equal(_standings.Count(), _dbSaveCalled);
        }

        Task<Standing> IDatabase<Standing>.GetBySourceId(int id)
        {
            throw new System.NotImplementedException();
        }

        async Task<IEnumerable<Standing>> IProvider<Standing>.RetrieveAll() => await Task.FromResult(_standings);

        void IDatabase<Standing>.Save(Standing fixture) => _dbSaveCalled++;
    }
}
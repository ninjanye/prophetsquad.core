using System;
using System.Linq;
using System.Threading.Tasks;

namespace ProphetSquad.Core.Matcher
{
    public class OddsMatcher
    {
        private readonly IFixtureProvider _fixtureProvider;
        private readonly IOddsProvider _oddsProvider;
        private readonly IFixturesDatabase _database;

        public OddsMatcher(IFixtureProvider fixtureProvider, IOddsProvider oddsProvider, IFixturesDatabase database)
        {
            _fixtureProvider = fixtureProvider;
            _oddsProvider = oddsProvider;
            _database = database;
        }

        public async Task Synchronise()
        {
            Console.WriteLine("Updating fixtures with odds...");
            var fixtures = await FixtureCollection.RetrieveFrom(_fixtureProvider);
            fixtures.UpdateOdds(_oddsProvider);
            fixtures.SaveTo(_database);
        }
    }
}
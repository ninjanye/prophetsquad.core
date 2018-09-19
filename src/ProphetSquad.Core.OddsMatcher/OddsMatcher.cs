using System;
using System.Linq;

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

        public void Synchronise()
        {
            Console.WriteLine("Updating fixtures with odds...");
            var fixtures = FixtureCollection.RetrieveFrom(_fixtureProvider);
            fixtures.UpdateOdds(_oddsProvider);
            fixtures.SaveTo(_database);
        }
    }
}
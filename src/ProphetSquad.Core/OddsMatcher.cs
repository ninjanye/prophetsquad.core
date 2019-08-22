using ProphetSquad.Core.Collections;
using ProphetSquad.Core.Databases;
using System;
using System.Threading.Tasks;

namespace ProphetSquad.Core
{
    public class OddsMatcher
    {
        private readonly IFixtureProvider fixtureProvider;
        private readonly IOddsProvider oddsSource;
        private readonly IFixturesDatabase fixtureDatabase;

        public OddsMatcher(IFixtureProvider fixtureProvider, IOddsProvider oddsSource, IFixturesDatabase fixtureDatabase)
        {
            this.fixtureProvider = fixtureProvider;
            this.oddsSource = oddsSource;
            this.fixtureDatabase = fixtureDatabase;
        }

        public async Task Synchronise()
        {
            var fixtures = await FixtureCollection.RetrieveFrom(fixtureProvider, DateTime.Today, DateTime.Today.AddDays(7));
            fixtures.UpdateOdds(oddsSource);
            fixtures.SaveTo(fixtureDatabase);
        }
    }
}
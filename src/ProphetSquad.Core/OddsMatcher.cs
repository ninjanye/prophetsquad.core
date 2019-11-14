using Microsoft.Extensions.Logging;
using ProphetSquad.Core.Collections;
using ProphetSquad.Core.Databases;
using ProphetSquad.Core.Providers;
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

        public async Task Synchronise(ILogger logger = null)
        {
            var fixtures = await FixtureCollection.RetrieveFrom(fixtureProvider, DateTime.Today, DateTime.Today.AddDays(7));
            fixtures.UpdateOdds(oddsSource, logger);
            fixtures.SaveTo(fixtureDatabase);
        }
    }
}
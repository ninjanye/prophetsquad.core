using Microsoft.Extensions.Logging;
using ProphetSquad.Core.Collections;
using ProphetSquad.Core.Data.Models;
using ProphetSquad.Core.Databases;
using ProphetSquad.Core.Providers;
using System;
using System.Threading.Tasks;

namespace ProphetSquad.Core
{
    public class OddsMatcher
    {
        private readonly IFixtureProvider fixtureProvider;
        private readonly IProvider<MatchOdds> oddsSource;
        private readonly IFixturesDatabase fixtureDatabase;

        public OddsMatcher(IFixtureProvider fixtureProvider, IProvider<MatchOdds> oddsSource, IFixturesDatabase fixtureDatabase)
        {
            this.fixtureProvider = fixtureProvider;
            this.oddsSource = oddsSource;
            this.fixtureDatabase = fixtureDatabase;
        }

        public async Task Synchronise(ILogger logger = null)
        {
            var fixtures = await FixtureCollection.RetrieveFrom(fixtureProvider, DateTime.UtcNow, DateTime.Today.AddDays(7));
            fixtures.UpdateOdds(oddsSource, logger);
            fixtures.SaveTo(fixtureDatabase);
        }
    }
}
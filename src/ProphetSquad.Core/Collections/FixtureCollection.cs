using Microsoft.Extensions.Logging;
using ProphetSquad.Core.Data.Models;
using ProphetSquad.Core.Databases;
using ProphetSquad.Core.Providers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProphetSquad.Core.Collections
{
    public class FixtureCollection : IEnumerable<Fixture>
    {
        private IEnumerable<Fixture> _fixtures;

        private FixtureCollection(IEnumerable<Fixture> fixtures)
        {
            _fixtures = fixtures;
        }

        public static async Task<FixtureCollection> RetrieveFrom(IFixtureProvider fixtureProvider, DateTime start, DateTime end)
        {
            IEnumerable<Fixture> fixtures = await fixtureProvider.Retrieve(start, end);
            return new FixtureCollection(fixtures);
        }

        public void UpdateOdds(IOddsProvider oddsProvider, ILogger logger = null)
        {
            var odds = OddsCollection.RetrieveFrom(oddsProvider).Result;
            int matchedOddsCount = 0;
            foreach (var fixture in _fixtures.Where(f => f.RequiresOdds))
            {
                var matchOdds = odds.FindFor(fixture);
                if (matchOdds != null)
                {
                    fixture.MatchOddsId = matchOdds.Id;
                    fixture.Competition.BookieId = Convert.ToInt64(matchOdds.CompetitionId);
                    fixture.HomeTeam.BookieName = matchOdds.HomeTeamName;
                    fixture.AwayTeam.BookieName = matchOdds.AwayTeamName;
                    matchedOddsCount++;
                }
            }
            logger?.LogInformation($"Odds matched: {matchedOddsCount}");
        }

        public void SaveTo(IDatabase<Fixture> database)
        {
            foreach (var fixture in this)
            {
                database.Save(fixture);
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public IEnumerator<Fixture> GetEnumerator() => _fixtures.GetEnumerator();
    }
}
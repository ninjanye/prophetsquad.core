using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProphetSquad.Core.Data.Models;

namespace ProphetSquad.Core.Matcher
{
    public class FixtureCollection : IEnumerable<Fixture>
    {
        private IEnumerable<Fixture> _fixtures;

        private FixtureCollection(IEnumerable<Fixture> fixtures)
        {
            _fixtures = fixtures;
        }

        public static async Task<FixtureCollection> RetrieveFrom(IFixtureProvider fixtureProvider)
        {
            DateTime today = DateTime.Today;
            IEnumerable<Fixture> fixtures = await fixtureProvider.Retrieve(today, today.AddDays(7));
            return new FixtureCollection(fixtures);
        }

        public void UpdateOdds(IOddsProvider oddsProvider)
        {
            var odds = OddsCollection.RetrieveFrom(oddsProvider).Result;
            foreach (var fixture in _fixtures.Where(f => f.RequiresOdds))
            {
                var matchOdds = odds.FindFor(fixture);
                if (matchOdds != null)
                {
                    fixture.MatchOddsId = matchOdds.Id;
                    fixture.Competition.BookieId = Convert.ToInt64(matchOdds.CompetitionId);
                    fixture.HomeTeam.BookieName = matchOdds.HomeTeamName;
                    fixture.AwayTeam.BookieName = matchOdds.AwayTeamName;
                }
            }
        }

        public void SaveTo(IFixturesDatabase database)
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
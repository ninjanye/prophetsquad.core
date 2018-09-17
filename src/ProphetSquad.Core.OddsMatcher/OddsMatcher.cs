using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ProphetSquad.Core.Data.Models;

namespace ProphetSquad.Core.Matcher
{
    public class OddsMatcher
    {
        private readonly IFixtureProvider _fixtureProvider;
        private readonly IOddsProvider _oddsProvider;

        public OddsMatcher(IFixtureProvider fixtureProvider, IOddsProvider oddsProvider)
        {
            _fixtureProvider = fixtureProvider;
            _oddsProvider = oddsProvider;
        }

        public void Synchronise()
        {
            //Get fixtures
            var fixtures = FixtureCollection.RetrieveFrom(_fixtureProvider);
            fixtures.UpdateOdds(_oddsProvider);

            //Get odds

            //Update fixtures with odds
            //Save fixtures

            //var fixtures = FixtureCollection.From(noOddsFixtureSource);
            //fixtures.UpdateWith(odds)
            //fixtures.SaveTo(database)
        }
    }

    internal class FixtureCollection : IEnumerable<Fixture>
    {
        private IEnumerable<Fixture> _fixtures;

        private FixtureCollection(IEnumerable<Fixture> fixtures)
        {
            _fixtures = fixtures;
        }

        internal static FixtureCollection RetrieveFrom(IFixtureProvider fixtureProvider)
        {
            return new FixtureCollection(fixtureProvider.Retrieve());
        }

        internal void UpdateOdds(IOddsProvider oddsProvider)
        {
            var odds = OddsCollection.RetrieveFrom(oddsProvider).Result;
            foreach (var fixture in _fixtures.Where(f => f.RequiresOdds()))
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

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IEnumerator<Fixture> GetEnumerator() => _fixtures.GetEnumerator();
    }
}
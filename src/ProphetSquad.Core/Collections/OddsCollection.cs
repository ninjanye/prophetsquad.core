using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProphetSquad.Core.Data.Models;
using ProphetSquad.Core.Databases;

namespace ProphetSquad.Core
{
    public interface IOddsCollection : IEnumerable<MatchOdds>
    {
        void SaveTo(IOddsDatabase database);
        MatchOdds FindFor(Fixture fixture);
    }

    public class OddsCollection : IOddsCollection
    {
        private readonly IEnumerable<MatchOdds> _odds;
        
        public static async Task<OddsCollection> RetrieveFrom(IOddsProvider source)
        {
            return new OddsCollection(await source.RetrieveAsync());
        }

        private OddsCollection(IEnumerable<MatchOdds> odds)
        {
            _odds = odds;
        }

        public void SaveTo(IOddsDatabase database)
        {
            var validOdds = _odds.Where(o => o.IsValid()).ToList();
            int i = 0;
            foreach (var matchOdds in validOdds)
            {
                Console.WriteLine($"Saving odds [{++i} of {validOdds.Count}] {matchOdds.HomeTeamName} vs {matchOdds.AwayTeamName}");
                database.Save(matchOdds);                
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IEnumerator<MatchOdds> GetEnumerator() => _odds.GetEnumerator();

        public MatchOdds FindFor(Fixture fixture)
        {
            Func<MatchOdds, bool> CompetitionsMatch = o => o.CompetitionId == fixture.Competition.BookieId.ToString() || o.CompetitionName.Equals(fixture.Competition?.Name, StringComparison.OrdinalIgnoreCase);
            Func<MatchOdds, bool> HomeTeamsMatch = o => o.HomeTeamName.Equals(fixture.HomeTeam.Name, StringComparison.OrdinalIgnoreCase) || o.HomeTeamName.Equals(fixture.HomeTeam.BookieName, StringComparison.OrdinalIgnoreCase);
            Func<MatchOdds, bool> AwayTeamsMatch = o => o.AwayTeamName.Equals(fixture.AwayTeam.Name, StringComparison.OrdinalIgnoreCase) || o.AwayTeamName.Equals(fixture.AwayTeam.BookieName, StringComparison.OrdinalIgnoreCase);
            Func<MatchOdds, bool> DatesMatch = o => o.Date >= fixture.Date && o.Date <= fixture.Date.AddHours(1);

            var odds = _odds.ToList();
            return odds.FirstOrDefault(o => CompetitionsMatch(o) && DatesMatch(o) && HomeTeamsMatch(o) && AwayTeamsMatch(o))
                ?? odds.FirstOrDefault(o => CompetitionsMatch(o) && DatesMatch(o) && HomeTeamsMatch(o))
                ?? odds.FirstOrDefault(o => CompetitionsMatch(o) && DatesMatch(o) && AwayTeamsMatch(o))
                ?? odds.FirstOrDefault(o => DatesMatch(o) && HomeTeamsMatch(o) && AwayTeamsMatch(o));
        }
    }
}

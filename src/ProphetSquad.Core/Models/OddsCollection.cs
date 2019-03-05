using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProphetSquad.Core.Data.Models;

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
            Func<MatchOdds, bool> CompetitionsMatch = o => o.CompetitionId == fixture.CompetitionId.ToString() || o.CompetitionName == fixture.Competition?.Name;
            Func<MatchOdds, bool> HomeTeamsMatch = o => o.HomeTeamId == fixture.HomeTeamId.ToString() || o.HomeTeamName == fixture.HomeTeam.BookieName;
            Func<MatchOdds, bool> AwayTeamsMatch = o => o.AwayTeamId == fixture.AwayTeamId.ToString();
            Func<MatchOdds, bool> DatesMatch = o => o.Date >= fixture.Date && o.Date <= fixture.Date.AddHours(1);

            var odds = _odds.ToList();
            return odds.FirstOrDefault(o => CompetitionsMatch(o) && DatesMatch(o) && HomeTeamsMatch(o) && AwayTeamsMatch(o))
                ?? odds.FirstOrDefault(o => CompetitionsMatch(o) && DatesMatch(o) && HomeTeamsMatch(o))
                ?? odds.FirstOrDefault(o => CompetitionsMatch(o) && DatesMatch(o) && AwayTeamsMatch(o));
        }
    }
}

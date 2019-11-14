using ProphetSquad.Core.Data.Models;
using ProphetSquad.Core.Databases;
using ProphetSquad.Core.Providers;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProphetSquad.Core
{
    public class StandingsCollection : IEnumerable<Standing>
    {
        private readonly IEnumerable<Standing> _standings;

        public static async Task<StandingsCollection> RetrieveFrom(IProvider<Standing> provider)
        {
            var standings = await provider.RetrieveAll();
            return new StandingsCollection(standings);
        }

        private StandingsCollection(IEnumerable<Standing> standings)
        {
            _standings = standings;
        }

        public async Task SaveTo(IDatabase<Standing> database, IDatabase<Competition> competitionDb, IDatabase<Team> teamDb)
        {
            foreach (var standing in _standings)
            {
                var comp = await competitionDb.GetBySourceId(standing.SourceCompetitionId);
                standing.CompetitionId = comp.Id;
                var team = await teamDb.GetBySourceId(standing.SourceTeamId);
                standing.TeamId = team.Id;
                database.Save(standing);
            }
        }

        public IEnumerator<Standing> GetEnumerator() => _standings.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
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

        public async Task SaveTo(IStore<Standing> database, IProvider<Competition> competitionDb, IProvider<Team> teamDb)
        {
            foreach (var standing in _standings)
            {
                var comp = await competitionDb.RetrieveBySourceId(standing.SourceCompetitionId);
                var team = await teamDb.RetrieveBySourceId(standing.SourceTeamId);

                if (comp != null && team!= null)
                {
                    standing.CompetitionId = comp.Id;
                    standing.TeamId = team.Id;
                    database.Save(standing);
                }
            }
        }

        public IEnumerator<Standing> GetEnumerator() => _standings.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
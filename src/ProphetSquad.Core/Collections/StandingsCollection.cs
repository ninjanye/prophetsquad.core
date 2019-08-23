using ProphetSquad.Core.Data.Models;
using ProphetSquad.Core.Databases;
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

        public void SaveTo(IDatabase<Standing> database)
        {
            foreach (var standing in _standings)
            {
                database.Save(standing);
            }
        }

        public IEnumerator<Standing> GetEnumerator() => _standings.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
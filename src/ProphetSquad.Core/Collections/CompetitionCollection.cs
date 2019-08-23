using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProphetSquad.Core.Data.Models;
using ProphetSquad.Core.Databases;

namespace ProphetSquad.Core.Collections
{
    public class CompetitionCollection : IEnumerable<Competition>
    {
        private IEnumerable<Competition> _competitions;

        private CompetitionCollection(IEnumerable<Competition> competitions)
        {
            _competitions = competitions;
        }

        public static async Task<CompetitionCollection> RetrieveFrom(IProvider<Competition> competitionProvider)
        {
            IEnumerable<Competition> competition = await competitionProvider.RetrieveAll();
            return new CompetitionCollection(competition);
        }

        public void SaveTo(IDatabase<Competition> database)
        {
            foreach (var competition in this)
            {
                database.Save(competition);
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public IEnumerator<Competition> GetEnumerator() => _competitions.GetEnumerator();
    }
}
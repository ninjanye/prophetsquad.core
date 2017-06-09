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
    }

    internal class OddsCollection : IOddsCollection
    {
        private readonly IEnumerable<MatchOdds> _odds;
        
        public static async Task<OddsCollection> RetrieveFrom(IOddsProvider source)
        {
            return new OddsCollection(await source.Retrieve());
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

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }        

        public IEnumerator<MatchOdds> GetEnumerator()
        {
            return _odds.GetEnumerator();
        }
    }
}

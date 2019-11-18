using ProphetSquad.Core.Data.Models;
using ProphetSquad.Core.Databases;
using ProphetSquad.Core.Providers;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProphetSquad.Core.Collections
{
    public class BetCollection : IEnumerable<Bet>
    {
        private readonly IEnumerable<Bet> _bets;
        private BetCollection(IEnumerable<Bet> bets)
        {
            _bets = bets;
        }

        public static async Task<BetCollection> RetrieveFrom(IProvider<Bet> source)
        {
           var bets = await source.RetrieveAll();
           return new BetCollection(bets);
        }

        public void SaveTo(IStore<Bet> database)
        {
            foreach (var bet in this)
            {
                database.Save(bet);
            }
        }

        public IEnumerator<Bet> GetEnumerator() => _bets.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
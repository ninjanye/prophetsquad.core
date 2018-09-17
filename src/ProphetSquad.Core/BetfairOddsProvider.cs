using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProphetSquad.Core.Data.Models;

namespace ProphetSquad.Core
{
    public class BetfairOddsProvider : IOddsProvider
    {
        private IBetfairClient _oddsSource;

        public BetfairOddsProvider(IBetfairClient oddsSource)
        {
            _oddsSource = oddsSource;
        }

        public async Task<IEnumerable<MatchOdds>> RetrieveAsync()
        {
            var odds = await _oddsSource.GetOdds();
            return odds.Select(MatchOdds.From);
        }
    }
}
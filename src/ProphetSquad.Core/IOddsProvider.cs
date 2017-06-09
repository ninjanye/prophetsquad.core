using System.Collections.Generic;
using System.Threading.Tasks;
using ProphetSquad.Core.Data.Models;

namespace ProphetSquad.Core
{
    public interface IOddsProvider
    {
        Task<IEnumerable<MatchOdds>> Retrieve();
    }
}
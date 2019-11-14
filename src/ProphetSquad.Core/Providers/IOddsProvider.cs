using System.Collections.Generic;
using System.Threading.Tasks;
using ProphetSquad.Core.Data.Models;

namespace ProphetSquad.Core.Providers
{
    public interface IOddsProvider
    {
        Task<IEnumerable<MatchOdds>> RetrieveAsync();
    }
}
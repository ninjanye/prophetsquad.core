using ProphetSquad.Core.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProphetSquad.Core.Providers
{
    public interface IOddsProvider
    {
        Task<IEnumerable<MatchOdds>> RetrieveAsync();
    }
}
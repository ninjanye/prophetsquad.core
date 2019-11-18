using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProphetSquad.Core.Providers
{
    public interface IProvider<T>
    {
        Task<IEnumerable<T>> RetrieveAll();
        Task<T> RetrieveBySourceId(int id);
    }
}
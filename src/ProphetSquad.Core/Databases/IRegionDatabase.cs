using ProphetSquad.Core.Data.Models;
using System.Threading.Tasks;

namespace ProphetSquad.Core.Databases
{
    public interface IRegionDatabase : IDatabase<Region>
    {
        Task<Region> Retrieve(string name);
    }

}
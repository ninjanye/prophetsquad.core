using ProphetSquad.Core.Data.Models;
using ProphetSquad.Core.Databases;
using System.Threading.Tasks;

namespace ProphetSquad.Core
{
    public interface IRegionDatabase : IDatabase<Region>
    {
        Task<Region> Retrieve(string name);
    }

}
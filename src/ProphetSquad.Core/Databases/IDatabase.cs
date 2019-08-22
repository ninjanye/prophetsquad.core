using System.Threading.Tasks;

namespace ProphetSquad.Core.Databases
{
    public interface IDatabase<T>
    {
        void Save(T fixture);
        Task<T> GetBySourceId(int id);

    }

}
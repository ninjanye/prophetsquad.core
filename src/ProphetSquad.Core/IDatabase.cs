using System.Threading.Tasks;

namespace ProphetSquad.Core
{
    public interface IDatabase<T>
    {
        void Save(T fixture);
        Task<T> GetBySourceId(int id);

    }

}
using System.Threading.Tasks;

namespace ProphetSquad.Core.Databases
{
    public interface IStore<T>
    {
        void Save(T item);
    }
}
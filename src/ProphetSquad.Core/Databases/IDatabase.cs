using ProphetSquad.Core.Providers;

namespace ProphetSquad.Core.Databases
{
    public interface IDatabase<T> : IStore<T>, IProvider<T>
    {        
    }
}
using ProphetSquad.Core.Data.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProphetSquad.Core
{
    public interface IDatabase<T>
    {
        void Save(T fixture);
        Task<T> GetBySourceId(int id);

    }

    public interface IFixturesDatabase : IDatabase<Fixture>
    {
        Task<IEnumerable<Fixture>> Retrieve(DateTime from, DateTime to);
    }

    public interface IGameweekDatabase
    {
        Task<Gameweek> Retrieve(DateTime date);
    }

    public interface IRegionDatabase : IDatabase<Region>
    {
        Task<Region> Retrieve(string name);
    }

}
using ProphetSquad.Core.Data.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProphetSquad.Core.Databases
{
    public interface IFixturesDatabase : IStore<Fixture>
    {
        Task<IEnumerable<Fixture>> Retrieve(DateTime from, DateTime to);
    }
}
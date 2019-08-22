using ProphetSquad.Core.Data.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProphetSquad.Core
{
    public interface IFixturesDatabase : IDatabase<Fixture>
    {
        Task<IEnumerable<Fixture>> Retrieve(DateTime from, DateTime to);
    }
}
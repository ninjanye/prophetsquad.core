using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProphetSquad.Core.Data.Models;

namespace ProphetSquad.Core
{
    public interface IFixtureProvider
    {
        Task<IEnumerable<Fixture>> Retrieve(DateTime from, DateTime to);
    }

    public interface IProvider<T>
    {
        Task<IEnumerable<T>> RetrieveAll();
    }
}
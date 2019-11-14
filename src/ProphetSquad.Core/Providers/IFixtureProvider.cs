using ProphetSquad.Core.Data.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProphetSquad.Core.Providers
{
    public interface IFixtureProvider
    {
        Task<IEnumerable<Fixture>> Retrieve(DateTime from, DateTime to);
    }
}
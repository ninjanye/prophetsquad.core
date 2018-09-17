using System.Collections.Generic;
using ProphetSquad.Core.Data.Models;

namespace ProphetSquad.Core
{
    public interface IFixtureProvider
    {
        IEnumerable<Fixture> Retrieve();
    }
}
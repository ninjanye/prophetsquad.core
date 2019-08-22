using ProphetSquad.Core.Data.Models;
using System;
using System.Threading.Tasks;

namespace ProphetSquad.Core
{
    public interface IGameweekDatabase
    {
        Task<Gameweek> Retrieve(DateTime date);
    }

}
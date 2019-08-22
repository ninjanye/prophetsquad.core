using ProphetSquad.Core.Data.Models;
using System;
using System.Threading.Tasks;

namespace ProphetSquad.Core.Databases
{
    public interface IGameweekDatabase
    {
        Task<Gameweek> Retrieve(DateTime date);
    }

}
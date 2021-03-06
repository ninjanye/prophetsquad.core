﻿using ProphetSquad.Core.Data.Models;
using ProphetSquad.Core.Databases;
using ProphetSquad.Core.Providers;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProphetSquad.Core.Collections
{
    public class TeamCollection : IEnumerable<Team>
    {
        private IEnumerable<Team> _teams;

        private TeamCollection(IEnumerable<Team> teams)
        {
            _teams = teams;
        }

        public static async Task<TeamCollection> RetrieveFrom(IProvider<Team> teamProvider)
        {
            IEnumerable<Team> teams = await teamProvider.RetrieveAll();
            return new TeamCollection(teams);
        }

        public void SaveTo(IStore<Team> database)
        {
            foreach (var team in this)
            {
                database.Save(team);
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public IEnumerator<Team> GetEnumerator() => _teams.GetEnumerator();
    }
}
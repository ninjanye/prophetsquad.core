using System.Collections.Generic;

namespace ProphetSquad.Core.Data.Models.ApiFootball
{
    public class LeaguesResponse
    {
        public int Results { get; set; }
        public List<League> Leagues { get; set; }
    }
}
using System.Collections.Generic;

namespace ProphetSquad.Core.Data.Models.ApiFootball
{
    public class MatchResponse
    {
        public int Count { get; set; }
        public List<Match> Fixtures { get; set; }
    }
}
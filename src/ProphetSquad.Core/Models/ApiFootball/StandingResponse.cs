using Newtonsoft.Json;

namespace ProphetSquad.Core.Data.Models.ApiFootball
{
    public class StandingResponse
    {
        public int Results { get; set; }
        public Standing[][] Standings { get; set; }
    }

    public class Standing
    {
        public int Rank { get; set; }
        [JsonProperty("team_id")]
        public int TeamId { get; set; }
        public string TeamName { get; set; }
        public string Group { get; set; }

        public Ranking All { get; set; }
        public int GoalsDiff { get; set; }
        public int Points { get; set; }
    }

    public class Ranking
    {
        [JsonProperty("matchsPlayed")]
        public int MatchesPlayed { get; set; }
        public int Win { get; set; }
        public int Draw { get; set; }
        public int Lose { get; set; }
        public int Points { get; set; }
        public int GoalsFor { get; set; }
        public int GoalsAgainst { get; set; }
    }
}
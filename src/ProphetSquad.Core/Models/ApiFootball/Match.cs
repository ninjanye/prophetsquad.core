using Newtonsoft.Json;
using System;

namespace ProphetSquad.Core.Data.Models.ApiFootball
{
    public class Match
    {
        [JsonProperty("fixture_id")]
        public int Id { get; set; }

        [JsonProperty("league_id")]
        public int LeagueId { get; set; }
        public League League { get; set; }

        [JsonProperty("event_timestamp")]        
        public long EventDate { get; set; }

        [JsonProperty("statusShort")]
        public string Status { get; set; }

        public Team HomeTeam { get; set; }
        public Team AwayTeam { get; set; }

        public int? GoalsHomeTeam { get; set; }
        public int? GoalsAwayTeam { get; set; }
    }
}
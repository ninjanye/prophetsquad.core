using Newtonsoft.Json;
using System;

namespace ProphetSquad.Core.Data.Models.FootballDataApi
{
    public class Match
    {
        public int Id { get; set; }
        public Competition Competition { get; set; }

        [JsonProperty("utcDate")]
        public DateTime Date { get; set; }
        public string Status { get; set; }

        public Team HomeTeam { get; set; }
        public Team AwayTeam { get; set; }

        public Result Score { get; set; }
    }
}
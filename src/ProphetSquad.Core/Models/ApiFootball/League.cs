using System;
using Newtonsoft.Json;

namespace ProphetSquad.Core.Data.Models.ApiFootball
{
    public class League
    {
        [JsonProperty("league_id")]
        public int LeagueId { get; set; }
        public string Name { get; set; }
        public string Country { get; set; }
        [JsonProperty("country_code")]
        public string CountryCode { get; set; }
        public int Season { get; set; }
        public Uri Logo { get; set; }
        public Uri Flag { get; set; }
    }
}
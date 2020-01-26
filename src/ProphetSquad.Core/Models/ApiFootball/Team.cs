using Newtonsoft.Json;

namespace ProphetSquad.Core.Data.Models.ApiFootball
{
    public class Team
    {
        [JsonProperty("team_id")]
        public int Id { get; set; }
        [JsonProperty("team_name")]
        public string Name { get; set; }
        public string Logo { get; set; }
    }
}
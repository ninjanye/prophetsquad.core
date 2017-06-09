using Newtonsoft.Json;

namespace ProphetSquad.Core.Models.Betfair.Response
{
    public class Team
    {
        public string SelectionId { get; set; }
        
        [JsonProperty("runnerName")]
        public string Name { get; set; }
        public Metadata Metadata { get; set; }
        public string Id => Metadata.Id;
        public decimal Odds { get; set; }
    }
}
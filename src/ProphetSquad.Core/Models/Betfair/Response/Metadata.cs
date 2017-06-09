using Newtonsoft.Json;

namespace ProphetSquad.Core.Models.Betfair.Response
{
    public class Metadata
    {
        [JsonProperty("runnerId")]
        public string Id { get; set; }
    }
}
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ProphetSquad.Core.Models.Betfair.Response
{
    public class MarketBook 
    {
        public string Status { get; set; }
        public string MarketId { get; set; }

        [JsonProperty("runners")]
        public List<TeamOdds> Teams { get; set; }
    }
}
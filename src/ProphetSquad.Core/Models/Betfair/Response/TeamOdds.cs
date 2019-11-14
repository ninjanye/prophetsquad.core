using Newtonsoft.Json;
using System.Linq;

namespace ProphetSquad.Core.Models.Betfair.Response
{
    public class TeamOdds
    {
        public string SelectionId { get; set; }

        [JsonPropertyAttribute("ex")]
        public Exchange Exchange { get; set; }

        public decimal Odds => Exchange.AvailableToBack.Any() ? Exchange.AvailableToBack[0].Price : 0m;
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace ProphetSquad.Core.Models.Betfair.Response
{
    public class Market
    {        
        [JsonProperty("marketId")]
        public string Id { get; set; }

        [JsonProperty("marketName")]
        public string Name { get; set; }

        [JsonProperty("marketStartTime")]
        public DateTime StartTime { get; set; }
        public Competition Competition { get; set; }
        
        [JsonProperty("runners")]
        public Team[] Teams { get; set; }

        public void PopulateOdds(List<MarketBook> odds)
        {            
            var marketBook = odds.SingleOrDefault(o => o.MarketId == Id);
            if(marketBook == null) return;

            var validTeams = Teams?.Where(t => !t.Name.Contains("The Draw"));
            var homeTeam = validTeams?.FirstOrDefault();
            var awayTeam = validTeams?.LastOrDefault();
            var homeTeamPrice = marketBook.Teams.SingleOrDefault(t => t.SelectionId == homeTeam.SelectionId);
            var awayTeamPrice = marketBook.Teams.SingleOrDefault(t => t.SelectionId == awayTeam.SelectionId);
            homeTeam.Odds = homeTeamPrice.Odds;
            awayTeam.Odds = awayTeamPrice.Odds;
        }
    }
}
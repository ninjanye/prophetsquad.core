using System.Collections.Generic;

namespace ProphetSquad.Core.Models.Betfair.Request
{
    internal class Filter
    {
        public ISet<string> EventTypeIds { get; set; }
        public ISet<string> MarketCountries { get; set; }
        public ISet<string> MarketTypeCodes { get; set; }
        public TimeRange MarketStartTime { get; set; }
    }
}
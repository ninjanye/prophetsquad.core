using System;

namespace ProphetSquad.Core.Data.Models
{
    public class BetfairOdds
    {
        public string Id { get; set; }
        public long CompetitionId { get; set; }
        public string CompetitionName { get; set; }
        public DateTime Date { get; set; }
        public long HomeTeamId { get; set; }
        public string HomeTeamName { get; set; }
        public string HomeTeamOdds { get; set; }
        public decimal HomeTeamOddsDecimal { get; set; }
        public long AwayTeamId { get; set; }
        public string AwayTeamName { get; set; }
        public string AwayTeamOdds { get; set; }
        public decimal AwayTeamOddsDecimal { get; set; }
    }
}

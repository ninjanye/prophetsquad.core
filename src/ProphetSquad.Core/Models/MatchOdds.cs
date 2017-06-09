using System;
using ProphetSquad.Core.Models.Betfair.Response;

namespace ProphetSquad.Core.Data.Models
{
    public class MatchOdds
    {
        public static MatchOdds From(Market source)
        {
            return new MatchOdds(source);
        }

        public MatchOdds(){            
        }

        private MatchOdds(Market source)
        {
            Id = source.Id;
            CompetitionId = source.Competition?.Id;
            CompetitionName = source.Competition?.Name;
            Date = source.StartTime;
            if(source.Teams?.Length > 1)
            {
                var homeTeam = source.Teams[0];
                HomeTeamId = homeTeam.Id;
                HomeTeamName = homeTeam.Name;
                HomeTeamOddsDecimal = homeTeam.Odds;

                var awayTeam = source.Teams[1];
                AwayTeamId = awayTeam.Id;                
                AwayTeamName = awayTeam.Name;
                AwayTeamOddsDecimal = awayTeam.Odds;
            }
            LastUpdate = DateTime.UtcNow;
        }
        
        public string Id { get; }
        public string CompetitionId { get; }
        public string CompetitionName { get; }
        public DateTime Date { get; }
        public string HomeTeamId { get; }
        public string HomeTeamName { get; }
        public string HomeTeamOdds => OddsConverter.ToFractional(HomeTeamOddsDecimal);
        public decimal HomeTeamOddsDecimal { get; }
        public string AwayTeamId { get; }
        public string AwayTeamName { get; set; }
        public string AwayTeamOdds => OddsConverter.ToFractional(AwayTeamOddsDecimal);
        public decimal AwayTeamOddsDecimal { get; set; }
        public DateTime LastUpdate { get; }
        public bool Processed { get; }

        public bool IsValid()
        {
            return 
                !String.IsNullOrEmpty(Id) &&
                !String.IsNullOrEmpty(CompetitionId) &&
                !String.IsNullOrEmpty(CompetitionName) &&
                Date != null &&
                !String.IsNullOrEmpty(HomeTeamId) &&
                !String.IsNullOrEmpty(HomeTeamName) &&
                !String.IsNullOrEmpty(HomeTeamOdds) &&
                HomeTeamOddsDecimal > 0 &&
                !String.IsNullOrEmpty(AwayTeamId) &&
                !String.IsNullOrEmpty(AwayTeamName) &&
                !String.IsNullOrEmpty(AwayTeamOdds) &&
                AwayTeamOddsDecimal > 0;
        }
    }
}

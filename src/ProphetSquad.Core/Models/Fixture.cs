using System;

namespace ProphetSquad.Core.Data.Models
{
    public class Fixture
    {
        public int Id { get; set; }
        public int OpenFootyId { get; set; }
        public int CompetitionId { get; set; }        
        public int GameweekId { get; set; }        
        public DateTime Date { get; set; }
        public int HomeTeamId { get; set; }        
        public int HomeTeamScore { get; set; }        
        public int AwayTeamId { get; set; }        
        public int AwayTeamScore { get; set; }        
        public int? WinnerId { get; set; }
        public bool IsResult { get; set; }
        public string MatchOddsId { get; set; }
        public bool Processed { get; set; }

        public Competition Competition { get; set; }
        public Team HomeTeam { get; set; }
        public Team AwayTeam { get; set; }

        public bool RequiresOdds()
        {
            return Date > DateTime.UtcNow && String.IsNullOrEmpty(MatchOddsId);
        }
    }

    public class Competition
    {
        public long BookieId { get; set; }
    }

    public class Team
    {
        public string BookieName { get; set; }
    }
}

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

        public int ModelState => 0;

        public bool RequiresOdds => Date > DateTime.UtcNow && String.IsNullOrEmpty(MatchOddsId);
    }
}

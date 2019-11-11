namespace ProphetSquad.Core.Data.Models
{
    public class Standing
    {
        public int SourceCompetitionId { get; set; }
        public int CompetitionId { get; set; }
        public int SourceTeamId { get; set; }
        public int TeamId { get; set; }
        public int Played { get; set; }
        public int Wins { get; set; }
        public int Draws { get; set; }
        public int Losses { get; set; }
        public int GoalsFor { get; set; }
        public int GoalsAgainst { get; set; }
        public int Points { get; set; }
        public string Form { get; set; }
    }
}

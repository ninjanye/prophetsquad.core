namespace ProphetSquad.Core.Data.Models.FootballDataApi
{
    public class Result
    {
        public string Winner { get; set; }
        public Score FullTime { get; set; }
        public Score HalfTime { get; set; }
    }
}
namespace ProphetSquad.Core.Data.Models
{
    public class Team
    {
        public int Id { get; set; }
        public int OpenFootyId { get; set; }
        public string Name { get; set; }
        public string SeoUrl => Regex.Replace(Name, @"[^A-Za-z0-9_\.~]+", "-").ToLowerInvariant();
        public string BookieName { get; set; }
        public string Badge { get; set; }
    }
}

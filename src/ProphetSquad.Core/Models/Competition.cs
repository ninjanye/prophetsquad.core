using System.Text.RegularExpressions;

namespace ProphetSquad.Core.Data.Models
{
    public class Competition
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string SeoUrl => Regex.Replace(Name, @"[^A-Za-z0-9_\.~]+", "-").ToLowerInvariant();
        public long BookieId { get; set; }
        public int OpenFootyId { get; set; }
        public int RegionId { get; set; }
    }
}

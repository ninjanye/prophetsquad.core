using System.Text.RegularExpressions;

namespace ProphetSquad.Core.Data.Models
{
    public class Region
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string SeoUrl => Regex.Replace(Name, @"[^A-Za-z0-9_\.~]+", "-").ToLowerInvariant();
        public string Code { get; set; }
    }
}

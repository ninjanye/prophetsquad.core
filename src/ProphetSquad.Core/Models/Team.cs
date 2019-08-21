using System;

namespace ProphetSquad.Core.Data.Models
{
    public class Team
    {
        public int Id { get; set; }
        public int OpenFootyId { get; set; }
        public string Name { get; set; }
        public string SeoUrl => Name.ToLower().Replace(" ", "-");
        public string BookieName { get; set; }
        public string Badge { get; set; }
    }
}

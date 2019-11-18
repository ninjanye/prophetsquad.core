namespace ProphetSquad.Core.Data.Models
{
    public class Competition
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public long BookieId { get; set; }
        public int OpenFootyId { get; set; }
        public int RegionId { get; set; }
    }
}

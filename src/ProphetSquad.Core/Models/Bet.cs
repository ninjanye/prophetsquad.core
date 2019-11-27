namespace ProphetSquad.Core.Data.Models
{
    public class Bet
    {
        public int TeamId { get; set; }
        public bool Processed { get; set; }
        public decimal OddsDecimal { get; set; }
        public int ModelState { get; set; }
        public User User { get; set; }

        public Fixture Fixture { get; set; }
        
    }

    public class User
    {
        public int Id { get; set; }
        public decimal Points { get; set; }
    }
}

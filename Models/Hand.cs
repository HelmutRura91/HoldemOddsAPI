namespace HoldemOddsAPI.Models
{
    public class Hand
    {
        public Card? Card1 { get; set; }
        public Card? Card2 { get; set; }

        public override string ToString()
        {
            return $"{Card1.ToString()} and {Card2.ToString()}";
        }
    }
}

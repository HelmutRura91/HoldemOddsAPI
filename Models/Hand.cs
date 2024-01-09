namespace HoldemOddsAPI.Models
{
    public class Hand
    {
        public Card Card1 { get; set; }
        public Card Card2 { get; set; }

        public override string ToString()
        {
            return $"{Card1.ToString()} and {Card2.ToString()}";
        }

        public bool IsSuited()
        {
            if (Card1.Suit == Card2.Suit) return true;
            return false;
        }

        public bool HasSpecificRanks(Rank rank1, Rank rank2)
        {
            if ((Card1.Rank == rank1 && Card2.Rank == rank2) 
                || (Card2.Rank == rank1 && Card1.Rank == rank2)) return true;
            return false;
        }
    }   
}

namespace HoldemOddsAPI.Models
{
    public enum HandRank
    {
        HighCard,
        Pair,
        TwoPair,
        ThreeOfAKind,
        Straight,
        Flush,
        FullHouse,
        FourOfAKind,
        StraightFlush,
        RoyalFlush
    }
    public class PokerHandRank
    {
        public HandRank Rank { get; set; }
        public List<Card> Cards { get; set; }

        public PokerHandRank(HandRank rank, List<Card> cards)
        {
            Rank = rank;
            Cards = cards;
        }
    }

}

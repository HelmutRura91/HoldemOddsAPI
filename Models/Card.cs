using System.Text.Json.Serialization;

namespace HoldemOddsAPI.Models
{
    public enum Suit
    {
        Hearts, // 0
        Diamonds,
        Clubs,
        Spades
    }

    public enum Rank
    {
        Two = 2,
        Three = 3,
        Four = 4,
        Five = 5,
        Six= 6,
        Seven = 7,
        Eight = 8,
        Nine = 9,
        Ten = 10,
        Jack = 11,
        Queen = 12,
        King = 13,
        Ace = 14
    }

    //implementing IComperable interface lets me sort a collection of Cards using LINQ, otherwise I should do sth like cards.OrderBy(card => card.Rank).ToList() instead of cards.OrderBy(card => card).ToList(). CompareTo method must be implemented when using IComperable
    public class Card : IComparable<Card>
    {
        public Suit Suit { get; }
        public Rank Rank { get; }

        public Card(Suit suit, Rank rank)
        {
            Suit = suit;
            Rank = rank;
        }
        //TODO REVIEW -> this constructor was used when I tried to deserialize json to PokerTable and to get all the info, but I failed
        [JsonConstructor]
        public Card(string suit, string rank)
        {
            Suit = ConvertStringToSuit(suit);
            Rank = ConvertStringToRank(rank);
        }

        public override string ToString()
        {
            return $"{Rank} of {Suit}";
        }

        public int CompareTo(Card other)
        {
            int rankComparison = this.Rank.CompareTo(other.Rank);
            if (rankComparison != 0) 
                return rankComparison;
            return 0;
        }

        //TODO -> REVIEW These were helper methods 
        private Suit ConvertStringToSuit(string suitString)
        {
            return suitString switch
            {
                "Hearts" => Suit.Hearts,
                "Diamonds" => Suit.Diamonds,
                "Clubs" => Suit.Clubs,
                "Spades" => Suit.Spades,
                _ => throw new ArgumentException("Invalid suit value")
            };
        }

        private Rank ConvertStringToRank(string rankString)
        {
            return rankString switch
            {
                "Two" => Rank.Two,
                "Three" => Rank.Three,
                "Four" => Rank.Four,
                "Five" => Rank.Five,
                "Six" => Rank.Six,
                "Seven" => Rank.Seven,
                "Eight" => Rank.Eight,
                "Nine" => Rank.Nine,
                "Ten" => Rank.Ten,
                "Jack" => Rank.Jack,
                "Queen" => Rank.Queen,
                "King" => Rank.King,
                "Ace" => Rank.Ace,
                _ => throw new ArgumentException("Invalid rank value")
            };
        }
    }

    

}

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
        public Suit Suit { get; set; }
        public Rank Rank { get; set; }
       
        //C# startup API parse String to ENUm
        //TODO REVIEW -> this constructor was used when I tried to deserialize json to PokerTable and to get all the info, but I failed

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
    }

    

}

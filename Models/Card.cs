using System.Collections;
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


    public class Card
    {
        public Suit? Suit { get; set; }
        public Rank? Rank { get; set; }
       
        public override string ToString()
        {
            return $"{Rank} of {Suit}";
        }
    }

    

}

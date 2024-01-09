using HoldemOddsAPI.Models;

namespace HoldemOddsAPI.Helpers
{
    public class PokerUtility
    {
        public static string GetHandDescription(PokerHandRank handRank)
        {
            return handRank.Rank switch
            {
                HandRank.RoyalFlush => "Royal Flush",
                HandRank.StraightFlush => "Straight Flush",
                HandRank.FourOfAKind => "Four of a Kind",
                HandRank.FullHouse => "Full House",
                HandRank.Flush => "Flush",
                HandRank.Straight => "Straight",
                HandRank.ThreeOfAKind => "Three of a Kind",
                HandRank.TwoPair => "Two Pair",
                HandRank.Pair => "Pair",
                HandRank.HighCard => "High Card",
                _ => "Unknown Hand",
            };
        }
    }
}

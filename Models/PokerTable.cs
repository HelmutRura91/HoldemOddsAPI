using System.Security.Cryptography.X509Certificates;

namespace HoldemOddsAPI.Models
{
    public class PokerTable
    {
        public List<Player> Players {get; private set;}
        public Deck Deck { get; internal set;}
        public List<Card> CommunityCards { get; private set;}
        public int Pot { get; internal set;}

        public PokerTable()
        {
            Players = new List<Player>();
            Deck = new Deck();
            CommunityCards = new List<Card>();
            Pot = 0;
        }
    }
}

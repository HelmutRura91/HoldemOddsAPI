using System;
using System.Collections.Generic;
using System.Linq;


namespace HoldemOddsAPI.Models
{
    public class Deck
    {
        private List<Card> cards;

        public Deck()
        {
            cards = Enumerable
                .Range(0,4) // Suits
                .SelectMany(s=> Enumerable
                    .Range(2,13) //Ranks
                    .Select(r=> new Card((Suit)s, (Rank)r)))
                .ToList();
            
            Shuffle();
        }

        public void Shuffle()
        {
            var rnd = new Random();
            int n = cards.Count;
            while (n > 1)
            {
                n--;
                int k = rnd.Next(n + 1);
                (cards[k], cards[n]) = (cards[n], cards[k]);
            }
        }

        public Hand DealHand()
        {
            if(cards.Count <2) throw new InvalidOperationException("Not enough cards to deal a hand.");
            var hand = new Hand(cards[0], cards[1]);
            cards.RemoveRange(0,2); //Remove the dealt cards from the deck
            return hand;
        }
    }
}

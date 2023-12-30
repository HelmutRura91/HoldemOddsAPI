using System;
using System.Collections.Generic;
using System.Linq;
using HoldemOddsAPI.Models;

namespace HoldemOddsAPI.Services
{
    public class DeckService
    {
        public void InitializeDeck(Deck deck)
        {
            deck.Cards = Enumerable
                    .Range(0, 4) // Suits
                    .SelectMany(s => Enumerable
                        .Range(2, 13) // Ranks
                        .Select(r => new Card((Suit)s, (Rank)r)))
                    .ToList();
        }
        
        public List<Card> GetAllCards(Deck deck)
        {
            return new List<Card>(deck.Cards);
        }
        public Hand DealHand(Deck deck)
        {
            if (deck.Cards.Count < 2)
                throw new InvalidOperationException("Not enough cards to deal a hand.");
            var hand = new Hand(deck.Cards[0], deck.Cards[1]);
            deck.Cards.RemoveRange(0,2);
            return hand;
        }
    }
        
}

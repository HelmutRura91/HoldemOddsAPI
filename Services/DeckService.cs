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
                        .Select(r => new Card { Suit = (Suit)s, Rank = (Rank)r }))
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
            var hand = new Hand { Card1 = deck.Cards[0], Card2 = deck.Cards[1] };
            deck.Cards.RemoveRange(0,2);
            return hand;
        }

        public void BurnCard(Deck deck)
        {
            if(deck.Cards.Count > 0)
            {
                deck.Cards.RemoveAt(0);
            }
            else
            {
                throw new InvalidOperationException("Cannot burn a card from an empy deck.");
            }
        }

        public List<Card> DealFlop(Deck deck)
        {
            BurnCard(deck);
            return DealCards(deck, 3);
        }

        public Card DealTurn (Deck deck)
        {
            BurnCard(deck);
            return DealCards(deck, 1).FirstOrDefault();
        }

        public Card DealRiver(Deck deck)
        {
            BurnCard(deck);
            return DealCards(deck, 1).FirstOrDefault();
        }

        private List<Card> DealCards(Deck deck, int numberOfCards)
        {
            if(deck.Cards.Count >= numberOfCards)
            {
                var cardsToDeal = deck.Cards.Take(numberOfCards).ToList();
                deck.Cards.RemoveRange(0, numberOfCards);
                return cardsToDeal;
            }
            else
            {
                throw new InvalidOperationException("Not enough cards in the deck to deal.");
            }
        }
    }
        
}

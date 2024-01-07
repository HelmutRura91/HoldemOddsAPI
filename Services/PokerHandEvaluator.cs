using HoldemOddsAPI.Models;

namespace HoldemOddsAPI.Services
{
    
    public class PokerHandEvaluator
    {
        public PokerHandRank EvaluateHand(IEnumerable<Card> cards)
        {
            if (IsRoyalFlush(cards, out var royalFlushHand))
                return new PokerHandRank { Rank = HandRank.RoyalFlush, Cards = royalFlushHand.ToList() };

            if (IsStraightFlush(cards, out var straightFlushHand))
                return new PokerHandRank{ Rank = HandRank.StraightFlush, Cards = straightFlushHand.ToList() };

            if (IsFourOfAKind(cards, out var fourOfAKindHand))
                return new PokerHandRank{ Rank = HandRank.FourOfAKind, Cards = fourOfAKindHand.ToList() };

            if (IsFullHouse(cards, out var fullHouseHand))
                return new PokerHandRank{ Rank = HandRank.FullHouse, Cards = fullHouseHand.ToList() };

            if (IsFlush(cards, out var flushHand))
                return new PokerHandRank{ Rank = HandRank.Flush, Cards = flushHand.ToList() };

            if (IsStraight(cards, out var straightHand))
                return new PokerHandRank{ Rank = HandRank.Straight, Cards = straightHand.ToList() };

            if (IsThreeOfAKind(cards, out var threeOfAKindHand))
                return new PokerHandRank{ Rank = HandRank.ThreeOfAKind, Cards = threeOfAKindHand.ToList() };

            if (IsTwoPair(cards, out var twoPairHand))
                return new PokerHandRank{ Rank = HandRank.TwoPair, Cards = twoPairHand.ToList() };

            if (IsPair(cards, out var pairHand))
                return new PokerHandRank{ Rank = HandRank.Pair, Cards = pairHand.ToList() };

            // Default to High Card if no other hand is formed
            var sortedCards = cards.OrderByDescending(card => card.Rank).ToList();
            return new PokerHandRank{ Rank = HandRank.HighCard, Cards = sortedCards };
        }

        public bool IsPair(IEnumerable<Card> cards, out IEnumerable<Card> bestHand)
        {
            bestHand = Enumerable.Empty<Card>();
            var groupWithPair = cards.GroupBy(card => card.Rank)
                .Where(group => group.Count() >= 2)
                .OrderByDescending(group => group.Key)
                .FirstOrDefault();

            if (groupWithPair == null) return false;

            var pair = groupWithPair.Take(2);
            var kickers = cards.Except(pair).OrderByDescending(card => card).Take(3);
            bestHand = pair.Concat(kickers);
            return true;
        }

        public bool IsTwoPair(IEnumerable<Card> cards, out IEnumerable<Card> bestHand)
        {
            bestHand = Enumerable.Empty<Card>();
            if (!IsPair(cards, out var firstPairHand)) return false;

            //Remove the cards of the first pair and search for a second pair
            var remainingCards = cards.Except(firstPairHand.Take(2));
            if(!IsPair(remainingCards, out var secondPairHand)) return false;

            var kicker = remainingCards.Except(secondPairHand.Take(2)).OrderByDescending(card => card).First();
            bestHand = firstPairHand.Take(2).Concat(secondPairHand.Take(2)).Concat(new[] {kicker });
            return true;
        }

        public bool IsThreeOfAKind(IEnumerable<Card> cards, out IEnumerable<Card> bestHand)
        {
            bestHand = Enumerable.Empty<Card>();
            var groupWithThree = cards.GroupBy(card => card.Rank)
                .Where(group => group.Count() == 3)
                .OrderByDescending(group => group.Key)
                .FirstOrDefault();

            if (groupWithThree == null) return false;

            var threeOfAKind = groupWithThree.Take(3);
            var kickers = cards.Except(threeOfAKind).OrderByDescending(card => card).Take(2);
            bestHand = threeOfAKind.Concat(kickers);
            return true;
        }

        public bool IsStraight(IEnumerable<Card> cards, out IEnumerable<Card> bestHand)
        {
            bestHand = Enumerable.Empty<Card>();
            var sortedCards = cards.OrderByDescending(card => card.Rank).DistinctBy(card => card.Rank).ToList();

            if (IsSequential(sortedCards))
            {
                bestHand = sortedCards.Take(5);
                return true;
            }

            //Check for Low Ace straight (Ace acting as '1')
            if (sortedCards.Any(card => card.Rank == Rank.Ace))
            {
                var lowAceCards = sortedCards.Select(card => card.Rank == Rank.Ace ? new Card { Suit = card.Suit, Rank = Rank.Two - 1 } : card).ToList();
                if (IsSequential(lowAceCards))
                {
                    bestHand = sortedCards.Take(5);
                    return true;
                }
            }
            return false;
        }

        public bool IsFlush(IEnumerable<Card> cards, out IEnumerable<Card> bestHand)
        {
            bestHand = Enumerable.Empty<Card>();
            var suitGroups = cards.GroupBy(card => card.Suit)
                .Where(group => group.Count() >= 5)
                .OrderByDescending(group => group.Max(card => card.Rank))
                .FirstOrDefault();

            if (suitGroups == null) return false;

            bestHand = suitGroups.OrderByDescending(card => card.Rank).Take(5);
            return true;
        }

        public bool IsFullHouse(IEnumerable<Card> cards, out IEnumerable<Card> bestHand)
        {
            bestHand = Enumerable.Empty<Card>();

            if(!IsThreeOfAKind(cards, out var threeOfAKindHand)) return false;

            var threeOfAKindCards = threeOfAKindHand.Take(3);

            var remainingCards = cards.Except(threeOfAKindCards);

            if(!IsPair(remainingCards, out var pairHand)) return false;

            var pairCards = pairHand.Take(2);

            bestHand = threeOfAKindCards.Concat(pairCards);
            return true;
        }

        public bool IsFourOfAKind(IEnumerable<Card> cards, out IEnumerable<Card> bestHand)
        {
            bestHand = Enumerable.Empty<Card>();
            var groupWithFour = cards.GroupBy(card => card.Rank)
                .Where(group => group.Count() == 4)
                .OrderByDescending(group => group.Key)
                .FirstOrDefault();

            if (groupWithFour == null) return false;

            var fourOfKind = groupWithFour.Take(4);
            var kicker = cards.Except(fourOfKind).OrderByDescending(card=>card.Rank).First();
            bestHand = fourOfKind.Concat(new[] { kicker });
            return true;
        }

        public bool IsStraightFlush(IEnumerable<Card> cards, out IEnumerable<Card> bestHand)
        {
            bestHand = Enumerable.Empty<Card>();
            var suitGroup = cards.GroupBy(card => card.Suit)
                .FirstOrDefault(group => group.Count() >= 5);
            
            if (suitGroup != null)
            {
                return IsStraight(suitGroup, out bestHand);
            }
            return false;
        }

        public bool IsRoyalFlush(IEnumerable<Card> cards, out IEnumerable<Card> bestHand)
        {
            bestHand = Enumerable.Empty<Card>();

            if (IsStraightFlush(cards, out var straightFlushHand))
            {
                if (straightFlushHand.Max(card => card.Rank) == Rank.Ace && straightFlushHand.Any(card => card.Rank == Rank.Ten))
                {
                    bestHand = straightFlushHand;
                    return true;
                }
            }
            return false;
        }

        private bool IsSequential(List<Card> cards)
        {
            int sequentialCount = 0;

            for (int i = 0; i < cards.Count - 1; i++)
            {
                if (cards[i].Rank - cards[i + 1].Rank == 1)
                {
                    sequentialCount++;
                    if (sequentialCount >= 4) // Four comparisons indicating a sequence of five card
                    {
                        return true;
                    }
                }
                else
                {
                    sequentialCount = 0; // Reset count if sequence is broken
                }
            }
            return false;
        }

        //    public Dictionary<Guid, double> CalculateWinningProbabilities(PokerTable table, DeckService deckService)
        //{
        //    var remainingDeck = deckService.GetAllCards(table.Deck);
        //    var currentBestHand = GetCurrentBestHand(table);
        //    var probabilities = new Dictionary<Guid, double>();

        //    foreach(var player in table.Players)
        //    {
        //        int beneficialCards = CountBeneficialCards(player, currentBestHand, remainingDeck);
        //        probabilities[player.Id] = (double)beneficialCards /remainingDeck.Count;
        //    }

        //    return probabilities;
        //}

        //private PokerHandRank GetCurrentBestHand(PokerTable table)
        //{
        //    //Evaluate and return the best hand from the current state of the table
        //}

        //private int CountBeneficialCards(Player player, PokerHandRank currentBestHand, Deck remainingDeck)
        //{
        //    //Count how many cards in the remaining deck would improve player's hand to beat the currentBestHand
        //}

        public int CompareHands(PokerHandRank hand1, PokerHandRank hand2)
        {
            if(hand1.Rank != hand2.Rank)
            {
                return hand1.Rank.CompareTo(hand2.Rank);
            }

            //Special case if Lower Ace in straight, straight flush
            if (hand1.Rank == HandRank.Straight || hand1.Rank == HandRank.StraightFlush)
            {
                bool hand1IsLowAceStraight = IsLowerAceStraight(hand1.Cards);
                bool hand2IsLowAceStraight = IsLowerAceStraight(hand2.Cards);

                if (hand1IsLowAceStraight && hand2IsLowAceStraight) return 0;
                if (hand1IsLowAceStraight) return -1;
                if (hand2IsLowAceStraight) return 1;
            }

            return CompareWholeHands(hand1.Cards, hand2.Cards);
        }

        private bool IsLowerAceStraight(IEnumerable<Card> cards)
        {
            return cards.Any(card => card.Rank == Rank.Ace) && cards.Any(card => card.Rank == Rank.Two);
        }

        private int CompareWholeHands(IEnumerable<Card> hand1,  IEnumerable<Card> hand2)
        {
            //EvaluateHand gives the PokerHandRank sorted in a specific way -> set cards are first, then kickers in descending way, so we mustn't perform sorting here
            //var sortedHand1 = hand1.OrderByDescending(c => c).ToList();
            //var sortedHand2 = hand2.OrderByDescending(c => c).ToList();
            var hand1List = hand1.ToList();
            var hand2List = hand2.ToList();

            for(int i =0; i < hand1List.Count; i++)
            {
                int comparison = hand1List[i].CompareTo(hand2List[i]);
                if (comparison != 0)
                {
                    return comparison;
                }
            }
            return 0;
        }

    }
}

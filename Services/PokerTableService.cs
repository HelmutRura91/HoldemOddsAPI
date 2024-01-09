using HoldemOddsAPI.Extensions;
using HoldemOddsAPI.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Collections.Generic;
using HoldemOddsAPI.Helpers;
using System.Xml.Linq;
using System;

namespace HoldemOddsAPI.Services
{
    public class PokerTableService
    {
        //private means that state of _pokerTable can only be manipulated through the methods provided by the service
        //readonly means that _pokerTable variable can only be assigned a value during its declaration or in the constructor of the class
        //private readonly PokerTable _pokerTable;
        private readonly GameState _gameState;
        private readonly DeckService _deckService;
        private readonly PokerHandEvaluator _handEvaluator;
        
        //changed the private field (_IsGameStarted) to a public property with a private setter, so PokerController could read the value
        //public bool IsGameStarted { get; private set; }

        public PokerTableService(GameState gameState, DeckService deckService, PokerHandEvaluator handEvaluator)
        {
            _gameState = gameState;
            _deckService = deckService;
            _handEvaluator = handEvaluator;
        }

        public int StartNewGame(int numberOfPlayers)
        {

            if (numberOfPlayers < 2 || numberOfPlayers > 9)
            {
                throw new ArgumentException("Number of players must be between 2 and 9.");
            }

            int gameId = _gameState.CreateNewGame();
            var pokerTable = _gameState.GetGame(gameId);
            _deckService.InitializeDeck(pokerTable.Deck);
            pokerTable.Deck.Shuffle();

            Random random = new Random();
            for (int i = 0; i < numberOfPlayers; i++)
            {
                pokerTable.Players.Add(new Player { Id = Guid.NewGuid(), Name = PlayerNames.Names[random.Next(PlayerNames.Names.Count)], ChipCount = 1000, IsFolded = false });
            }
            _gameState.AddOrUpdateGame(gameId, pokerTable);
            return gameId;
        }

        public Dictionary<Player, Hand> DealInitialHands(int gameId)
        {
            var pokerTable = _gameState.GetGame(gameId);
            if (pokerTable == null)
            {
                throw new InvalidOperationException("The game has not been started.");
            }

            var playerHands = new Dictionary<Player, Hand>();
            foreach (var player in pokerTable.Players)
            {
                var hand = _deckService.DealHand(pokerTable.Deck);
                player.SetHand(hand);
                playerHands.Add(player, hand);
            }
            return playerHands;
        }
            
            
        public List<Card> DealFlop(int gameId)
        {
            var pokerTable = _gameState.GetGame(gameId);
            if (pokerTable == null)
            {
                throw new InvalidOperationException("The game has not been started.");
            }

            if (pokerTable.CommunityCards.Any())
            {
                throw new InvalidOperationException("Flop can only be dealt once.");
            }

            var flopCards = _deckService.DealFlop(pokerTable.Deck);
            pokerTable.CommunityCards.AddRange(flopCards);
            return flopCards;
        }

        public Card DealTurn(int gameId)
        {
            var pokerTable = _gameState.GetGame(gameId);
            if (pokerTable == null)
            {
                throw new InvalidOperationException("The game has not been started.");
            }

            if (pokerTable.CommunityCards.Count != 3)
            {
                throw new InvalidOperationException("Turn can only be dealt after the flop.");
            }

            var turnCard = _deckService.DealTurn(pokerTable.Deck);
            pokerTable.CommunityCards.Add(turnCard);
            return turnCard;
        }

        public Card DealRiver(int gameId)
        {
            var pokerTable = _gameState.GetGame(gameId);
            if (pokerTable == null)
            {
                throw new InvalidOperationException("The game has not been started.");
            }

            if (pokerTable.CommunityCards.Count != 4)
                throw new InvalidOperationException("Turn can only be dealt after the turn.");

            var riverCard = _deckService.DealRiver(pokerTable.Deck);
            pokerTable.CommunityCards.Add(riverCard);
            return riverCard;
        }

        public IEnumerable<object> GetFormattedPlayerHands(int gameId)
        {
            var pokerTable = _gameState.GetGame(gameId);
            if (pokerTable == null)
            {
                throw new InvalidOperationException("Game not found");
            }

            //The new keyword is used to create an instance of an anonymous type.Anonymous types provide a convenient way to encapsulate a set of read-only properties into a single object without having to explicitly define a type first.
            return pokerTable.Players.Select(player => new
            {
                PlayerId = player.Id,
                PlayerName = player.Name,
                Hand = player.GetFormattedHand(),

               // HandRank = PokerUtility.GetHandDescription(player.CurrentHandRank)
            });
        }

        public IEnumerable<object> GetFormattedPlayerHandsWithProbabilities(int gameId, Dictionary<Guid, double> winningProbabilities)
        {
            var pokerTable = _gameState.GetGame(gameId);
            if (pokerTable == null)
            {
                throw new InvalidOperationException("Game not found");
            }

            return pokerTable.Players.Select(player =>
            {
                winningProbabilities.TryGetValue(player.Id, out double probability);
                var formattedProbability = $"{probability * 100:0.00}%";
                return new
                {
                    PlayerId = player.Id,
                    PlayerName = player.Name,
                    Hand = player.GetFormattedHand(),
                    Probability = formattedProbability
                };              
            });
        }


        public List<Card> GetCommunityCards(int gameId)
        {
            var pokerTable = _gameState.GetGame(gameId);
            if (pokerTable == null)
            {
                throw new InvalidOperationException("Game not found.");
            }

            return pokerTable.CommunityCards;
        }

        public void EvaluatePlayersHand(int gameId)
        {
            var pokerTable = _gameState.GetGame(gameId);
            if (pokerTable == null)
            {
                throw new InvalidOperationException("Game not found.");
            }

            var communityCards = pokerTable.CommunityCards;

            foreach (var player in pokerTable.Players)
            {
                var playerCards = new List<Card> { player.CurrentHand.Card1, player.CurrentHand.Card2 };
                var combinedHand = playerCards.Concat(communityCards);

                player.CurrentHandRank = _handEvaluator.EvaluateHand(combinedHand);
            }
        }

        public Player DetermineWinningHand(int gameId)
        {
            var pokerTable = _gameState.GetGame(gameId);
            if(pokerTable == null)
            {
                throw new InvalidOperationException("Game not found");
            }

            Player winningPlayer = null;
            PokerHandRank winningHandRank = null;

            foreach (var player in pokerTable.Players)
            {
                if(!player.IsFolded && (winningPlayer == null || _handEvaluator.CompareHands(player.CurrentHandRank, winningHandRank) > 0))
                {
                    winningPlayer = player;
                    winningHandRank = player.CurrentHandRank;
                }
            }
            //TODO Add condition when tied
            return winningPlayer;
        }

        // BRUTE FORCE METHOD FOR CALCULATING PROBABILITIES, REQUIRES SIMULATING ALL TURN AND RIVER CARDS
        // CODE IS DUPLICATED BECAUSE I HAVE TO PASS PokerTable instance instead of gameId which is associated with ActiveGames and created during CreateGame

        public List<Card> GetCommunityCardsForSimulation(PokerTable pokerTable)
        {
            if (pokerTable == null)
            {
                throw new ArgumentNullException(nameof(pokerTable), "Poker table cannot be null.");
            }

            return new List<Card>(pokerTable.CommunityCards);
        }

        public void EvaluatePlayersHandForSimulation(PokerTable pokerTable)
        {
            if (pokerTable == null)
            {
                throw new ArgumentNullException(nameof(pokerTable), "Poker table cannot be null.");
            }

            foreach (var player in pokerTable.Players)
            {
                var playerCards = new List<Card> { player.CurrentHand.Card1, player.CurrentHand.Card2 };
                var combinedHand = playerCards.Concat(pokerTable.CommunityCards);
                player.CurrentHandRank = _handEvaluator.EvaluateHand(combinedHand);
            }
        }

        public Player DetermineWinningHandForSimulation(PokerTable pokerTable)
        {
            if (pokerTable == null)
            {
                throw new ArgumentNullException(nameof(pokerTable), "Poker table cannot be null.");
            }

            Player winningPlayer = null;
            PokerHandRank winningHandRank = null;

            foreach (var player in pokerTable.Players)
            {
                if (!player.IsFolded && (winningPlayer == null || _handEvaluator.CompareHands(player.CurrentHandRank, winningHandRank) > 0))
                {
                    winningPlayer = player;
                    winningHandRank = player.CurrentHandRank;
                }
            }
            return winningPlayer;
        }

        public Dictionary<Guid, double> CalculateWinningProbabilities(int gameId)
        {
            var pokerTable = _gameState.GetGame(gameId);
            if (pokerTable == null || pokerTable.CommunityCards.Count < 3)
            {
                throw new InvalidOperationException("Game not found or not enough community cards dealt");
            }

            var probabilities = new Dictionary<Guid, double>();
            var allDeckCards = _deckService.GetAllCards(pokerTable.Deck);
            var cardsInPlay = pokerTable.CommunityCards
                .Concat(pokerTable.Players.SelectMany(p => new List<Card> { p.CurrentHand.Card1, p.CurrentHand.Card2 }));
            var remainingDeck = allDeckCards.Except(cardsInPlay).ToList();
            var totalScenarios = 0;

            if(pokerTable.CommunityCards.Count == 4)
            {
                foreach (var riverCard in remainingDeck)
                {
                    totalScenarios++;
                    // Create a deep copy of PokerTable for each scenario
                    var simulationPokerTable = new PokerTable
                    {
                        Players = pokerTable.Players.Select(p => p.Clone()).ToList(),
                        CommunityCards = new List<Card>(pokerTable.CommunityCards) { riverCard },
                        Deck = new Deck { Cards = pokerTable.Deck.Cards.ToList() }
                    };
                    EvaluatePlayersHandForSimulation(simulationPokerTable);
                    var winningPlayer = DetermineWinningHandForSimulation(simulationPokerTable);

                    UpdateProbabilities(winningPlayer, probabilities);
                    
                }
            }
            else //If only flop is dealt
            {
                foreach (var turnCard in remainingDeck)
                {
                    foreach (var riverCard in remainingDeck.Where(c => c != turnCard))
                    {
                        totalScenarios++;
                        // Create a deep copy of PokerTable for each scenario
                        var simulationPokerTable = new PokerTable
                        {
                            Players = pokerTable.Players.Select(p => p.Clone()).ToList(),
                            CommunityCards = new List<Card>(pokerTable.CommunityCards) { turnCard, riverCard },
                            Deck = new Deck { Cards = pokerTable.Deck.Cards.ToList() }
                        };

                        EvaluatePlayersHandForSimulation(simulationPokerTable);
                        var winningPlayer = DetermineWinningHandForSimulation(simulationPokerTable);

                        UpdateProbabilities(winningPlayer, probabilities);
                    }
                }
            }
            

            // Convert win counts to probabilities
            foreach (var player in pokerTable.Players)
            {
                probabilities[player.Id] = probabilities.ContainsKey(player.Id) ? probabilities[player.Id] / totalScenarios : 0;
            }

            return probabilities;
        }

        // Helper method to update probabilities
        private void UpdateProbabilities(Player winningPlayer, Dictionary<Guid, double> probabilities)
        {
            if (winningPlayer != null)
            {
                if (!probabilities.ContainsKey(winningPlayer.Id))
                {
                    probabilities[winningPlayer.Id] = 0;
                }
                probabilities[winningPlayer.Id]++;
            }
        }



        public void UpdateGameWithLoadedState(int gameId, PokerTable loadedPokerTable)
        {
            _gameState.AddOrUpdateGame(gameId, loadedPokerTable);
        }

    }
}

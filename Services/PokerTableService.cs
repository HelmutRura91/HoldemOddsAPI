using HoldemOddsAPI.Extensions;
using HoldemOddsAPI.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Collections.Generic;

namespace HoldemOddsAPI.Services
{
    public class PokerTableService
    {
        //private means that state of _pokerTable can only be manipulated through the methods provided by the service
        //readonly means that _pokerTable variable can only be assigned a value during its declaration or in the constructor of the class
        //private readonly PokerTable _pokerTable;
        private readonly GameState _gameState;
        private readonly DeckService _deckService;
        
        //changed the private field (_IsGameStarted) to a public property with a private setter, so PokerController could read the value
        //public bool IsGameStarted { get; private set; }

        public PokerTableService(GameState gameState, DeckService deckService)
        {
            _gameState = gameState;
            _deckService = deckService;
        }

        public int StartNewGame(int numberOfPlayers)
        {
            if (numberOfPlayers < 2 || numberOfPlayers > 9)
            {
                throw new ArgumentException("Number of players must be between 2 and 9.");
            }

            int gameId = _gameState.CreateNewGame();
            var pokerTable = _gameState.GetGame(gameId);

            pokerTable.Deck = new Deck();
            _deckService.InitializeDeck(pokerTable.Deck);
            pokerTable.Deck.Shuffle();
            pokerTable.Players.Clear();
            pokerTable.CommunityCards.Clear();
            pokerTable.Pot = 0;

            for(int i= 0; i < numberOfPlayers; i++)
            {
                pokerTable.Players.Add(new Player());
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
                Hand = player.GetFormattedHand()
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

        public void UpdateGameWithLoadedState(int gameId, PokerTable loadedPokerTable)
        {
            _gameState.AddOrUpdateGame(gameId, loadedPokerTable);
        }
        //public void HandlePlayerAction(Player player, PlayerAction action)
        //{
        //    // handle player actions (bet, fold, raise)
        //}

        //public Player DetermineWinner()
        //{

        //}
    }
}

using HoldemOddsAPI.Extensions;
using HoldemOddsAPI.Models;

namespace HoldemOddsAPI.Services
{
    public class PokerTableService
    {
        //private means that state of _pokerTable can only be manipulated through the methods provided by the service
        //readonly means that _pokerTable variable can only be assigned a value during its declaration or in the constructor of the class
        private readonly PokerTable _pokerTable;
        private readonly DeckService _deckService;
        
        //changed the private field (_IsGameStarted) to a public property with a private setter, so PokerController could read the value
        public bool IsGameStarted { get; private set; }

        public PokerTableService(DeckService deckService)
        {
            _pokerTable = new PokerTable();
            _deckService = deckService;
        }

        public void StartNewGame(int numberOfPlayers)
        {
            if (numberOfPlayers < 2 || numberOfPlayers > 9)
            {
                throw new ArgumentException("Number of players must be between 2 and 9.");
            }

            _pokerTable.Deck = new Deck();
            _deckService.InitializeDeck(_pokerTable.Deck);
            _pokerTable.Deck.Shuffle();
            _pokerTable.Players.Clear();
            _pokerTable.CommunityCards.Clear();
            _pokerTable.Pot = 0;

            for(int i= 0; i < numberOfPlayers; i++)
            {
                _pokerTable.Players.Add(new Player());
            }

            IsGameStarted = true;

        }

        public Dictionary<Player, Hand> DealInitialHands()
        {
            if (!IsGameStarted)
            {
                throw new InvalidOperationException("The game has not been started.");
            }

            var playerHands = new Dictionary<Player, Hand>();
            foreach (var player in _pokerTable.Players)
            {
                var hand = _deckService.DealHand(_pokerTable.Deck);
                player.SetHand(hand);
                playerHands.Add(player, hand);
            }
            return playerHands;
        }
            
            
        public void PlayFlop()
        {
            //deal the flop after the first round of betting
        }

        public void PlayTurn()
        {

        }

        public void PlayRiver()
        {

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

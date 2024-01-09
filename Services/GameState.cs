using HoldemOddsAPI.Models;

namespace HoldemOddsAPI.Services
{
    public class GameState
    {
        private int _nextGameId = 1;
        public Dictionary<int, PokerTable> ActiveGames { get; private set; }
        public GameState()
        {
            ActiveGames = new Dictionary<int, PokerTable>();
        }

        public int CreateNewGame()
        {
            int gameId = _nextGameId++;
            var pokerTable = new PokerTable { Players = new List<Player>(), Deck = new Deck(), CommunityCards = new List<Card>(), Pot = new int() };
            ActiveGames[gameId] = pokerTable;
            return gameId;
        }

        public void AddOrUpdateGame(int gameId, PokerTable pokerTable)
        {
            ActiveGames[gameId] = pokerTable;
        }
        public PokerTable GetGame(int gameId)
        {
            return ActiveGames.ContainsKey(gameId) ? ActiveGames[gameId] : null;
        }

        public void RemoveGame(int gameId) { }
    }
}

using HoldemOddsAPI.Models;
using System.Text.Json;

namespace HoldemOddsAPI.Services
{
    public class GameStateService
    {
        private readonly GameState _gameState;

        public GameStateService(GameState gameState)
        {
            _gameState = gameState;
        }

        public string SerializeGameState(int gameId)
        {
            var pokerTable = _gameState.GetGame(gameId);
            if (pokerTable == null)
            {
                throw new InvalidOperationException("Game not found.");
            }
            return JsonSerializer.Serialize(pokerTable, new JsonSerializerOptions
            {
                WriteIndented = true // For better readability when saving to a file
            });
        }

        public void SaveGameStateToFile(int gameId, string filePath)
        {
            string gameStateJson = SerializeGameState(gameId);
            File.WriteAllText(filePath, gameStateJson);
        }

        public PokerTable DeserializeGameState(string gameStateJson)
        {
            return JsonSerializer.Deserialize<PokerTable>(gameStateJson);
        }

        public PokerTable LoadGameStateFromFile(string filePath)
        {
            string gameStateJson = File.ReadAllText(filePath);
            return DeserializeGameState(gameStateJson);
        }
    }
}

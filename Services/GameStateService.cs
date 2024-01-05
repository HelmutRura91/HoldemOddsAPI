using HoldemOddsAPI.DataTransferObjects;
using HoldemOddsAPI.Models;
using System.Text.Json;

namespace HoldemOddsAPI.Services
{
    public class GameStateService
    {
        private readonly GameState _gameState;
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonSerializerOptions;

        public GameStateService(GameState gameState, HttpClient httpClient, JsonSerializerOptions jsonSerializerOptions)
        {
            _gameState = gameState;
            _httpClient = httpClient ?? new HttpClient();
            _jsonSerializerOptions = jsonSerializerOptions ?? new JsonSerializerOptions 
            { 
                PropertyNameCaseInsensitive = true,
                AllowTrailingCommas = true,
            };
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

        //TODO learn about async and Task
        public async Task<LoadGameResponse> LoadGameFromUrl(string url)
        {
            // Step 1: Fetch JSON from URL
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            //Console.WriteLine(json);

            // Step 2: Deserialize JSON to player data
            List<Player> players = new List<Player>();
            var jsonDocOptions = new JsonDocumentOptions
            {
                AllowTrailingCommas = true,
            };
            using (var doc = JsonDocument.Parse(json, jsonDocOptions))
            {
                var playersJson = doc.RootElement.GetProperty("players").EnumerateArray();
                foreach (var playerJson in playersJson)
                {
                    string name = "";
                    int chipCount = 0;

                    if(playerJson.TryGetProperty("Name", out JsonElement nameElement))
                    {
                        name = nameElement.GetString();
                    }

                    if (playerJson.TryGetProperty("ChipCount", out JsonElement chipCountElement))
                    {
                        chipCount = chipCountElement.GetInt32();
                    }


                    var player = new Player()
                    {
                        Name = name,
                        ChipCount = chipCount
                        //TODO REVIEW Using GetProperty didn't work -> probably because it it tried to access properties in the JSON that didn't exist
                        //Name = playerJson.GetProperty("Name").GetString(),
                        //ChipCount = playerJson.GetProperty("ChipCount").GetInt32()
                    };
                    players.Add(player);
                }
            }
            //I couldn't resolve problems with deserializing cards -> I got enums and in Json there are strings, 
            //ERROR MSG:Each parameter in the deserialization constructor on type 'HoldemOddsAPI.Models.Card' must bind to an object property or field on deserialization. Each parameter name must match with a property or field on the object. Fields are only considered when 'JsonSerializerOptions.IncludeFields' is enabled. The match can be case-insensitive.
            //var pokerTable = JsonSerializer.Deserialize<PokerTable>(json, _jsonSerializerOptions);

            // Step 3: Process PokerTable data
            var highestStack = FindHighestStack(players);
            var lowestStack = FindLowestStack(players);

            int entryStackThreshold = 1000;
            var (playersAboveEntryStack, playersBehindEntryStack) = CategorizePlayers(players, entryStackThreshold);

            return new LoadGameResponse
            {
                HighestStack = highestStack,
                LowestStack = lowestStack,
                ListOfPlayersAboveEntryStack = playersAboveEntryStack,
                ListOfPlayersBehindEntryStack = playersBehindEntryStack
            };

        }

        private PlayerStackInfo FindHighestStack(List<Player> players)
        {
            return players
                .Select(player => new PlayerStackInfo { Name = player.Name, Value = player.ChipCount })
                .OrderByDescending(info => info.Value)
                .FirstOrDefault();
        }

        private PlayerStackInfo FindLowestStack(List<Player> players)
        {
            return players
                .Select(player => new PlayerStackInfo { Name = player.Name, Value = player.ChipCount })
                .OrderBy(info => info.Value)
                .FirstOrDefault();
        }

        private (List<string>, List<string>) CategorizePlayers(List<Player> players, int entryStackThreshold)
        {
            var playersAbove = players
                .Where(player => player.ChipCount > entryStackThreshold)
                .Select(player => player.Name)
                .ToList();

            var playerBelow = players
                .Where(player => player.ChipCount <= entryStackThreshold)
                .Select(player => player.Name)
                .ToList();

            return(playersAbove, playerBelow);
        }
    }
}

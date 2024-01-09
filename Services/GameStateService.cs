using HoldemOddsAPI.Adapters;
using HoldemOddsAPI.DataTransferObjects;
using HoldemOddsAPI.Models;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace HoldemOddsAPI.Services
{
    public class GameStateService
    {
        private readonly GameState _gameState;
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonSerializerOptions;

        public GameStateService(GameState gameState, HttpClient httpClient)
        {
            _gameState = gameState;
            _httpClient = httpClient ?? new HttpClient();
            _jsonSerializerOptions = new JsonSerializerOptions
            {
                Converters = { new JsonStringEnumConverter() },
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

            // Use PokerTableAdapter
            PokerTableAdapter adapter = new PokerTableAdapter();
            PokerTable pokerTable = adapter.DeserializeAndValidate(json, _jsonSerializerOptions);

            // Step 2: Deserialize JSON to PokerTable Data
            //PokerTable pokerTable = JsonSerializer.Deserialize<PokerTable>(json, _jsonSerializerOptions);

            // Step 3: Process PokerTable data
            int entryStack = FindStartingStack(pokerTable);
            var (playersAboveEntryStack, playersBehindEntryStack) = CategorizePlayers(pokerTable, entryStack);

            return new LoadGameResponse
            {
                HighestStack = FindHighestStack(pokerTable),
                LowestStack = FindLowestStack(pokerTable),
                EntryStack = entryStack,
                ListOfPlayersAboveEntryStack = playersAboveEntryStack,
                ListOfPlayersBehindEntryStack = playersBehindEntryStack,
                SuperFolksCount = FindSuperFolksCount(pokerTable)
            };

        }
        private int FindStartingStack(PokerTable pokerTable)
        {
            int totalStack = (int)pokerTable.Players.Sum(player => player.ChipCount);
            int startingStack = totalStack / pokerTable.Players.Count();
            return startingStack;
        }

        private PlayerStackInfo FindHighestStack(PokerTable pokerTable)
        {
            var highestStackPlayer = pokerTable.Players.MaxBy(player => player.ChipCount);
            return new PlayerStackInfo { Name = highestStackPlayer.Name, Value = highestStackPlayer.ChipCount };
        }

        private PlayerStackInfo FindLowestStack(PokerTable pokerTable)
        {
            var lowestStackPlayer = pokerTable.Players.MinBy(player => player.ChipCount);
            return new PlayerStackInfo { Name = lowestStackPlayer.Name, Value = lowestStackPlayer.ChipCount };
        }

        private (List<string>, List<string>) CategorizePlayers(PokerTable pokerTable, int entryStack)
        {
            var playersAbove = pokerTable.Players
                .Where(player => player.ChipCount > entryStack)
                .Select(player => player.Name)
                .ToList();

            var playerBelow = pokerTable.Players
                .Where(player => player.ChipCount <= entryStack)
                .Select(player => player.Name)
                .ToList();

            return(playersAbove, playerBelow);
        }

        //superFolks are Players that plays a hand of 2,7 offsuit
        //assumption - if Card2 has no Suit or Rank, then it duplicates Card1 Suit or Rank
        private int FindSuperFolksCount(PokerTable pokerTable)
        {
            var superFolksCount = pokerTable.Players
                .Count(player => !player.CurrentHand.IsSuited() && player.CurrentHand.HasSpecificRanks((Rank)2, (Rank)7));

            return superFolksCount;
        }
    }
}

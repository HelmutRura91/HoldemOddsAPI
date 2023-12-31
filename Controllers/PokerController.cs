﻿using Microsoft.AspNetCore.Mvc;
using HoldemOddsAPI.Models;
using HoldemOddsAPI.Services;
using System.Text.Json;

namespace HoldemOddsAPI.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class PokerController : ControllerBase
    {
        //injecting PokerTableService into PokerController constructor
        private readonly PokerTableService _pokerTableService;
        private readonly GameStateService _gameStateService;
        private readonly JsonLogger _jsonLogger;
        
        public PokerController(PokerTableService pokerTableService, GameStateService gameStateService, JsonLogger jsonLogger)
        {
            _pokerTableService = pokerTableService;
            _gameStateService = gameStateService;
            _jsonLogger = jsonLogger;
        }

        [HttpPost("start-game/{numberOfPlayers}")]
        public IActionResult StartGame(int numberOfPlayers)
        {
            try
            {
                int gameId = _pokerTableService.StartNewGame(numberOfPlayers);
                return Ok(new
                {
                    Message = $"Game started with {numberOfPlayers} players.",
                    GameId = gameId
                });
            }
            catch (Exception ex)
            {
                _jsonLogger.LogError(ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("save-game/{gameId}")]
        public IActionResult SaveGame(int gameId)
        {
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string fileName = $"GameState_{gameId}_{timestamp}.json";
            string filePath = $@"C:\Users\npotu\source\repos\HoldemOddsAPI\SavedFiles\{fileName}";

            try
            {
                _gameStateService.SaveGameStateToFile(gameId, filePath);
                return Ok($"Game state for game {gameId} save successfully at {filePath}.");
            }
            catch (Exception ex)
            {
                _jsonLogger.LogError(ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("load-game/{gameId}")]
        public IActionResult LoadGame(int gameId, [FromBody] PokerTable pokerTable)
        {
            try
            {
                string gameStateJson = JsonSerializer.Serialize(pokerTable);
                var loadedPokerTable = _gameStateService.DeserializeGameState(gameStateJson);
                if (loadedPokerTable == null)
                {
                    return BadRequest("Invalid game state JSON");
                }

                // Update the game state in PokerTableService
                _pokerTableService.UpdateGameWithLoadedState(gameId, loadedPokerTable);

                return Ok("Game state loaded successfully.");
            }
            catch (Exception ex)
            {
                _jsonLogger.LogError(ex);
                return BadRequest($"Error loading game state: {ex.Message}");
            }
        }

        [HttpGet("deal-initial-hands/{gameId}")]
        public IActionResult DealInitialHands(int gameId)
        {
            try
            {
                var playerHands = _pokerTableService.DealInitialHands(gameId);
                var handsInfo = _pokerTableService.GetFormattedPlayerHands(gameId);
                return Ok(new 
                { 
                    Message = "Initial hands dealt to all players.",
                    Hands = handsInfo
                });
            }
            catch (Exception ex)
            {
                _jsonLogger.LogError(ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("deal-flop/{gameId}")]
        public IActionResult DealFlop(int gameId)
        {
            try
            {
                var flopCards = _pokerTableService.DealFlop(gameId);
                var handsInfo = _pokerTableService.GetFormattedPlayerHands(gameId);
                return Ok(new
                {
                    Flop = flopCards.Select(c => c.ToString()),
                    Hands = handsInfo
                });
            }
            catch (Exception ex)
            {
                _jsonLogger.LogError(ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("deal-turn/{gameId}")]
        public IActionResult DealTurn(int gameId)
        {
            try
            {
                var turnCard = _pokerTableService.DealTurn(gameId);
                var communityCards = _pokerTableService.GetCommunityCards(gameId);
                var handsInfo = _pokerTableService.GetFormattedPlayerHands(gameId);
                return Ok(new 
                { 
                    CommunityCards = communityCards.Select(c=>c.ToString()),
                    Hands = handsInfo                
                });
            }
            catch (Exception ex)
            {
                _jsonLogger.LogError(ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("deal-river/{gameId}")]
        public IActionResult DealRiver(int gameId)
        {
            try
            {
                var riverCard = _pokerTableService.DealRiver(gameId);
                var communityCards = _pokerTableService.GetCommunityCards(gameId);
                var handsInfo = _pokerTableService.GetFormattedPlayerHands(gameId);
                return Ok(new 
                { 
                    CommunityCards = communityCards.Select(c=>c.ToString()),
                    Hands = handsInfo
                });
            }
            catch (Exception ex)
            {
                _jsonLogger.LogError(ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("test-hand/{suit1}/{rank1}/{suit2}/{rank2}")]
        public IActionResult GetTestHand(Suit suit1, Rank rank1, Suit suit2, Rank rank2)
        {
            try
            {
                var card1 = new Card(suit1, rank1);
                var card2 = new Card(suit2, rank2);
                var hand = new Hand(card1, card2);
                return Ok(hand.ToString());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}

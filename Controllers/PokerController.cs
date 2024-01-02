using Microsoft.AspNetCore.Mvc;
using HoldemOddsAPI.Models;
using HoldemOddsAPI.Services;

namespace HoldemOddsAPI.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class PokerController : ControllerBase
    {
        //injecting PokerTableService into PokerController constructor
        private readonly PokerTableService _pokerTableService;
        public PokerController(PokerTableService pokerTableService)
        {
            _pokerTableService = pokerTableService;
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
                return BadRequest(ex.Message);
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

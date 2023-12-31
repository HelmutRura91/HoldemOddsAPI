using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using HoldemOddsAPI.Models;
using HoldemOddsAPI.Services;
using HoldemOddsAPI.Extensions;

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


        [HttpGet("start-and-deal/{numberOfPlayers}")]
        public IActionResult StartGameAndDealHands(int numberOfPlayers)
        {
            try
            {
                // Start the new game
                _pokerTableService.StartNewGame(numberOfPlayers);

                // Deal initial hands
                var playerHands = _pokerTableService.DealInitialHands();
                var handsInfo = playerHands.Select(ph => new
                {
                    Player = ph.Key.Id,
                    Hand = ph.Value.ToString()
                });

                return Ok(new
                {
                    Message = $"Game started with {numberOfPlayers} players and initial hands dealt.",
                    Hands = handsInfo
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("start-game/{numberOfPlayers}")]
        public IActionResult StartGame(int numberOfPlayers)
        {
            try
            {
                _pokerTableService.StartNewGame(numberOfPlayers);
                return Ok($"Game started with {numberOfPlayers} players.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("deal-initial-hands")]
        public IActionResult DealInitialHands()
        {
            if (!_pokerTableService.IsGameStarted)
            {
                return BadRequest("Game has not bedd started.");
            }
            try
            {
                var playerHands = _pokerTableService.DealInitialHands();
                var handsInfo = playerHands.Select(ph => new
                {
                    Player = ph.Key.Id,
                    Hands = ph.Value.ToString()
                });
                return Ok(new { Message = "Initial hands dealt to all players.", Hands = handsInfo});
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

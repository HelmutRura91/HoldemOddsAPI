using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using HoldemOddsAPI.Models;

namespace HoldemOddsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PokerController : ControllerBase
    {
        [HttpGet("test-hand")]
        public IActionResult GetTestHand()
        {
            var card1 = new Card(Suit.Clubs, Rank.Jack);
            var card2 = new Card(Suit.Clubs, Rank.Seven);
            var hand = new Hand(card1, card2);
            return Ok(hand.ToString());
        }
    }
}

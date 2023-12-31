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
        private readonly DeckService _deckService;
        public PokerController(DeckService deckService)
        {
            _deckService = deckService;
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

        [HttpGet("shuffle")]
        public IActionResult ShuffleDeck()
        {
            var deck = new Deck();
            _deckService.InitializeDeck(deck);
            deck.Shuffle();
            var cardStrings = _deckService.GetAllCards(deck).Select(card=>card.ToString()).ToList();
            return Ok(cardStrings);
        }

        [HttpGet("deal")]
        public IActionResult DealHand()
        {
            try
            {
                var deck = new Deck();
                _deckService.InitializeDeck(deck);
                deck.Shuffle();
                var hand = _deckService.DealHand(deck);
                return Ok(hand.ToString());
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }            
        }

    }
}

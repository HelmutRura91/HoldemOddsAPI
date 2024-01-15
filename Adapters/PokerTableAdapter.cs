using HoldemOddsAPI.Models;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace HoldemOddsAPI.Adapters
{
    //TODO Czy w tym podejściu brakuje mi jeszcze stworzenia nowego modelu PokerTable? Ale nie czuję tego.
    public class PokerTableAdapter
    {
        public PokerTable DeserializeAndValidate(string json, JsonSerializerOptions options)
        {
            PokerTable pokerTable = JsonSerializer.Deserialize<PokerTable>(json, options);
            ValidatePokerTable(pokerTable);
            return pokerTable;
        }

        private void ValidatePokerTable(PokerTable? pokerTable)
        {
            if (pokerTable == null)
            {
                throw new InvalidOperationException("Poker table data is null.");
            }

            if (pokerTable.Players == null || !pokerTable.Players.Any())
            {
                throw new InvalidOperationException("Players list is null or empty.");
            }

            pokerTable.Players = pokerTable.Players?
                .Where(player => IsValidPlayer(player))
                .ToList() ?? new List<Player>();

            foreach (var player in pokerTable.Players)
            {
                if (player.CurrentHand != null) DuplicateCardPropertiesIfNeeded(player.CurrentHand.Card1, player.CurrentHand.Card2);
            }

            // Validation for Deck and CommunityCards can be added here if needed            
        }

        private bool IsValidPlayer(Player player)
        {
            player.ChipCount = player.ChipCount ?? 0;

            return player != null
                && !string.IsNullOrWhiteSpace(player.Name)
                && player.Id != Guid.Empty;
                //&& player.CurrentHand != null
                //&& player.CurrentHand.Card1 != null
                //&& player.CurrentHand.Card2 != null;     
        }

        private void DuplicateCardPropertiesIfNeeded(Card card1, Card card2)
        {
            if(!card1.Suit.HasValue && card2.Suit.HasValue) { card1.Suit = card2.Suit; }

            else if (!card2.Suit.HasValue && card1.Suit.HasValue) { card2.Suit = card1.Suit; }

            if (!card1.Rank.HasValue && card2.Rank.HasValue) { card1.Rank = card2.Rank; }

            else if (!card2.Rank.HasValue && card1.Rank.HasValue) { card2.Rank = card1.Rank; }
        }
    }

}

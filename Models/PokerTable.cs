using System.Security.Cryptography.X509Certificates;
using System.Text.Json.Serialization;


namespace HoldemOddsAPI.Models
{
    public class PokerTable
    {
        [JsonPropertyName("players")]
        public List<Player> Players {get; set;}

        [JsonPropertyName("deck")]
        public Deck Deck { get; set;}

        [JsonPropertyName("communityCards")]
        public List<Card> CommunityCards { get; set;}

        [JsonPropertyName("pot")]
        public int? Pot { get; set;}

    }
}

namespace HoldemOddsAPI.Models
{
    public class Hand
    {
        public Card Card1 { get; set; }
        public Card Card2 { get; set; }

        public Hand(Card card1, Card card2)
        {
            Card1 = card1 ?? throw new ArgumentNullException(nameof(card1));
            Card2 = card2 ?? throw new ArgumentNullException(nameof(card2));
        }

        public override string ToString()
        {
            return $"{Card1.ToString()} and {Card2.ToString()}";
        }
    }
}

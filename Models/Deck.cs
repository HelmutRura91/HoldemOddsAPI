
namespace HoldemOddsAPI.Models
{
    public class Deck
    {
        //internat set to allow modification within the assembly
        public List<Card> Cards { get; internal set; }

        public Deck()
        {
            Cards = new List<Card>();
        }
    }
}

        

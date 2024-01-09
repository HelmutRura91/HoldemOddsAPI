
namespace HoldemOddsAPI.Models
{
    public class Deck
    {
        //internat set to allow modification within the assembly
        //had to change it to public so I could Deserialize it
        public List<Card> Cards { get; set; }
    }
}

        

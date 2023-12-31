using HoldemOddsAPI.Models;
using System.Runtime.CompilerServices;

namespace HoldemOddsAPI.Extensions
{
    public static class ExtensionMethods
    {
        // The first parameter takes the "this" modifier and specifies the type for which the method is defined.
        public static void Shuffle(this Deck deck)
        {
            var rnd = new Random();
            deck.Cards = deck.Cards.OrderBy(card => rnd.Next()).ToList();
        }
    }
}

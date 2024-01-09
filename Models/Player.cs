using HoldemOddsAPI.Helpers;

namespace HoldemOddsAPI.Models
{
    public class Player
    {
        // I had to make the setters public, so I could deserialize JSON data into Player
        
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Hand CurrentHand { get; set; }
        public PokerHandRank CurrentHandRank { get; set; }
        public int ChipCount { get; set; }
        public bool IsFolded { get; set; }

        // Parameterless constructor
        //public Player()
        //{
        //    Id = Guid.NewGuid();
        //    Name = PlayerNames.Names[random.Next(PlayerNames.Names.Count)];
        //    ChipCount = 1000;
        //    IsFolded = false;
        //}

        // Parameterized constructor that calls the parameterless constructor 

        public void SetHand(Hand hand)
        {
            if(hand == null)
                throw new ArgumentNullException(nameof(hand));
            CurrentHand = hand;
        }

        public void SetHandRank(PokerHandRank handRank)
        {
            if (handRank == null)
                throw new ArgumentNullException(nameof(handRank));
            CurrentHandRank = handRank;
        }

        public void Fold()
        {
            IsFolded = true;
        }

        public string GetFormattedHand()
        {
            var handDescription = CurrentHand != null ? CurrentHand.ToString() : "Hand not set.";
            var rankDescription = CurrentHandRank != null ? PokerUtility.GetHandDescription(CurrentHandRank) : "Rank not evaluated";
            return $"{handDescription} - {rankDescription}";
        }

        public Player Clone()
        {
            return new Player { Id = Id, Name = Name, CurrentHand = CurrentHand, ChipCount = ChipCount, IsFolded = IsFolded };
        }
        //Id = Guid.NewGuid(), Name = PlayerNames.Names[random.Next(PlayerNames.Names.Count)], CurrentHand = new Hand { Card1 = CurrentHand.Card1, Card2 = CurrentHand.Card2 },ChipCount = 1000, IsFolded = IsFolded
    }
}

namespace HoldemOddsAPI.Models
{
    public class Player
    {
        private static readonly Random random = new Random();
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public Hand CurrentHand { get; private set; }
        public int ChipCount { get; set; }
        public bool IsFolded { get; private set; }

        // Parameterless constructor
        public Player()
        {
            Id = Guid.NewGuid();
            Name = PlayerNames.Names[random.Next(PlayerNames.Names.Count)];
            ChipCount = 1000;
            IsFolded = false;
        }

        // Parameterized constructor that calls the parameterless constructor 
        public Player(Guid id, string name, Hand currentHand, int chipCount, bool isFolded) : this() //Calls the parameterless constructor first
        {
            Id = id;
            Name = name;
            CurrentHand = currentHand;
            ChipCount = chipCount;
            IsFolded = isFolded;
        }

        public void SetHand(Hand hand)
        {
            if(hand == null)
                throw new ArgumentNullException(nameof(hand));
            CurrentHand = hand;
        }

        public void Fold()
        {
            IsFolded = true;
        }

        public string GetFormattedHand()
        {
            return CurrentHand != null ? CurrentHand.ToString() : "Hand not set.";
        }
    }
}

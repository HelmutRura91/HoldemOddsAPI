namespace HoldemOddsAPI.Models
{
    public class Player
    {
        public Guid Id { get; private set; }
        public Hand CurrentHand { get; private set; }
        public int ChipCount { get; set; }
        public bool IsFolded { get; private set; }

        // Parameterless constructor
        public Player()
        {
            Id = Guid.NewGuid();
            ChipCount = 1000;
            IsFolded = false;
        }

        // Parameterized constructor that calls the parameterless constructor 
        public Player(Guid id, Hand currentHand, int chipCount, bool isFolded) : this() //Calls the parameterless constructor first
        {
            Id = id;
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
    }
}

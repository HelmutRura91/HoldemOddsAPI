using HoldemOddsAPI.Models;

namespace HoldemOddsAPI.Services
{
    public class PlayerService
    {
        private readonly List<Player> _players;

        public PlayerService()
        {
            _players = new List<Player>();
        }

        public void AddPlayer(Player player)
        {
            _players.Add(player);
        }

        public void RemovePlayer(Guid playerId)
        {
            _players.RemoveAll(p => p.Id == playerId);
        }

        public Player GetPlayer(Guid playerId)
        {
            return _players.Find(p => p.Id == playerId);
        }

        public void Fold(Guid playerId)
        {
            var player = GetPlayer(playerId);
            if (player != null)
            {
                player.Fold();
            }
        }

        //TODO add checking and raising

        public void UpdatePlayerHand(Guid playerId, Hand hand)
        {
            var player = GetPlayer(playerId);
            if (player != null)
            {
                player.SetHand(hand);
            }
        }

        public IEnumerable<Player> GetActivePlayers()
        {
            return _players.FindAll(p => !p.IsFolded);
        }
        
    }
}

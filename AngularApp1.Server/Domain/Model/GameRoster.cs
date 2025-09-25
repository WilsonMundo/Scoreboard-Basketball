namespace AngularApp1.Server.Domain.Model
{
    public class GameRoster
    {
        public long Id { get; set; }
        public long GameId { get; set; }
        public long TeamId { get; set; } // → TeamCatalog.Id
        public long PlayerId { get; set; } // → Player.Id


        public Game Game { get; set; } = default!;
        public TeamCatalog Team { get; set; } = default!;
        public Player Player { get; set; } = default!;
    }
}

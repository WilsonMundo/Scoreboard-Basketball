namespace AngularApp1.Server.Domain.Model
{
    public class GameTeam
    {
        public long Id { get; set; }
        public long GameId { get; set; }
        public long TeamId { get; set; } // → TeamCatalog.Id
        public bool IsHome { get; set; }
        public int Score { get; set; }
        public int Fouls { get; set; }


        public Game Game { get; set; } = default!;
        public TeamCatalog Team { get; set; } = default!;
    }
}

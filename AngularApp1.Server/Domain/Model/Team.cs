namespace AngularApp1.Server.Domain.Model
{
    public class Team
    {
        public long Id { get; set; }
        public long GameId { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsHome { get; set; }  // true = local, false = visitante
        public int Score { get; set; }    // útil para el siguiente slice
        public int FoulsTeam { get; set; }
        public string? LogoUrl { get; set; }

        public Game Game { get; set; } = default!;
    }
}

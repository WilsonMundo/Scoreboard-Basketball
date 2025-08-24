namespace AngularApp1.Server.Application.DTO.Game
{
    public class GameListItemDto
    {
        public long Id { get; set; }
        public string Status { get; set; } = "Active";
        public int Quarter { get; set; }
        public string? HomeName { get; set; }
        public string? AwayName { get; set; }
        public int HomeScore { get; set; }
        public int AwayScore { get; set; }
        public DateTime CreatedAtUtc { get; set; }
    }
}

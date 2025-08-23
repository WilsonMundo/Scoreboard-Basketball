namespace AngularApp1.Server.Application.DTO.Game
{
    public record CreateGameDto(string HomeName, string AwayName, int QuarterSeconds);
    public class GameDto
    {
        public long Id { get; set; }
        public string Status { get; set; } = "Active";
        public int Quarter { get; set; }
        public int QuarterSeconds { get; set; }
        public int RemainingSeconds { get; set; }
        public bool IsTimerRunning { get; set; }
        public TeamDto? Home { get; set; }
        public TeamDto? Away { get; set; }
        public IReadOnlyList<QuarterPointsDto> Box { get; set; } = Array.Empty<QuarterPointsDto>();
    }
    public class TeamDto
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsHome { get; set; }
        public int Score { get; set; }
        public int FoulsTeam { get; set; }
        public int Fouls { get; set; }
        public string? LogoUrl { get; set; }
    }

}

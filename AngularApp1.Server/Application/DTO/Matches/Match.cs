namespace AngularApp1.Server.Application.DTO.Matches
{
    public class Match
    {
        public record CreateMatchDto(long HomeTeamId, long AwayTeamId, DateTime? StartAtUtc, int QuarterSeconds = 600);


        public record AssignRosterDto(long TeamId, IReadOnlyList<long> PlayerIds);


        public class MatchHistoryItemDto
        {
            public long Id { get; set; }
            public string Status { get; set; } = string.Empty;
            public DateTime CreatedAtUtc { get; set; }
            public string? HomeName { get; set; }
            public string? AwayName { get; set; }
            public int HomeScore { get; set; }
            public int AwayScore { get; set; }
        }
    }
}

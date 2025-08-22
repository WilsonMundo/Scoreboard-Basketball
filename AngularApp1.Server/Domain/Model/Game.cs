namespace AngularApp1.Server.Domain.Model
{
    public class Game
    {
        public long Id { get; set; }
        public string Status { get; set; } = "Active";
        public int Quarter { get; set; } = 1;
        public int QuarterSeconds { get; set; } = 600;
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    }
}
}

namespace AngularApp1.Server.Domain.Model
{
    public class Game
    {
        public long Id { get; set; }
        public string Status { get; set; } = "Active";
        public int Quarter { get; set; } = 1;
        public int QuarterSeconds { get; set; } = 600;
        public int RemainingSeconds { get; set; } = 600;
        public bool IsTimerRunning { get; set; } = false;
        public DateTime? TimerStartedAtUtc { get; set; } // para calcular el tiempo transcurrido
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public ICollection<Team> Teams { get; set; } = new List<Team>();
        public ICollection<QuarterScore> QuarterScores { get; set; } = new List<QuarterScore>();

    }
}


namespace AngularApp1.Server.Domain.Model
{
    public class QuarterScore
    {
        public long Id { get; set; }

        public long GameId { get; set; }
        public long TeamId { get; set; }
        public int QuarterNumber { get; set; }   // 1..N (OTs = 5,6,...)
        public bool IsOvertime { get; set; }     // opcional (lo dejaremos, util para UI)
        public int Points { get; set; }          // >= 0

        public Game Game { get; set; } = default!;
        public Team Team { get; set; } = default!;
    }
}

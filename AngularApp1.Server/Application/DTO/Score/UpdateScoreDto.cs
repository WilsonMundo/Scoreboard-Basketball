namespace AngularApp1.Server.Application.DTO.Score
{
    public class UpdateScoreDto
    {
        public long TeamId { get; set; }
        public int DeltaPoints { get; set; } // -1, +1, +2, +3
    }
}

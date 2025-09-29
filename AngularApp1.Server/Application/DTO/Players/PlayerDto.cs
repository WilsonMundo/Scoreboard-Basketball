namespace AngularApp1.Server.Application.DTO.Players
{
    public class PlayerDto
    {
        public long Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public int? Number { get; set; }
        public string? Position { get; set; }
        public decimal? HeightMeters { get; set; }
        public int? Age { get; set; }
        public string? Nationality { get; set; }
        public long? TeamId { get; set; }
        public string? TeamName { get; set; }
        public DateTime CreatedAtUtc { get; set; }
    }
}

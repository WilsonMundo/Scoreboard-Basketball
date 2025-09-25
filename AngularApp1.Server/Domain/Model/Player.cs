namespace AngularApp1.Server.Domain.Model
{
    public class Player
    {
        public long Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public int? Number { get; set; }
        public string? Position { get; set; }
        public decimal? HeightMeters { get; set; }
        public int Age { get; set; }
        public string? Nationality { get; set; }
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public long? TeamId { get; set; }
        public TeamCatalog? Team { get; set; }
    }
}

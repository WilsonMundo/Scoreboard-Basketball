namespace AngularApp1.Server.Domain.Model
{
    public class TeamCatalog
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty; // único
        public string? City { get; set; }
        public string? LogoUrl { get; set; }
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    }
}

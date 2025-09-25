namespace AngularApp1.Server.Application.DTO.Teams
{
    public class TeamDto
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? City { get; set; }
        public string? LogoUrl { get; set; }
        public DateTime CreatedAtUtc { get; set; }
    }
}

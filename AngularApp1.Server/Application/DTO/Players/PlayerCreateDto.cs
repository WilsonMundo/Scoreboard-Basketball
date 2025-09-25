namespace AngularApp1.Server.Application.DTO.Players
{
    public record PlayerCreateDto(
            string FullName,
            int? Number,
            string? Position,
            decimal? HeightMeters,
            int? Age,
            string? Nationality,
            long? TeamId // opcional
            );



    public record PlayerUpdateDto(
    string FullName,
    int? Number,
    string? Position,
    decimal? HeightMeters,
    int? Age,
    string? Nationality,
    long? TeamId
    );


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


    public class PlayerListRequest
    {
        public string? Q { get; set; }
        public long? TeamId { get; set; }
        public int Page { get; set; } = 1;
        public int Size { get; set; } = 20;
        public string? Sort { get; set; } = "name"; // name|-name|created|-created|number|-number|age|-age
    }
}

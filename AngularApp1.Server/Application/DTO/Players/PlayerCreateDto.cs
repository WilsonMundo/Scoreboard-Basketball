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
}


namespace AngularApp1.Server.Application.DTO.Game
{
    public record CreateGameDto(string HomeName, string AwayName, int QuarterSeconds);
    public record GameDto(Guid Id, string Status, int Quarter, int QuarterSeconds);
}

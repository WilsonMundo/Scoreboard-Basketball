namespace AngularApp1.Server.Application.DTO.Teams
{
   
    public record TeamCreateDto(string Name, string? City);
    public record TeamUpdateDto(string Name, string? City);
}

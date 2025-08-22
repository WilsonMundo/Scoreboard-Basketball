using AngularApp1.Server.Application.DTO.Game;

namespace AngularApp1.Server.Application.Interfaces
{
    public interface IGameService
    {
        Task<GameDto> CreateAsync(CreateGameDto dto);
        Task<GameDto?> GetAsync(long id);
    }
}

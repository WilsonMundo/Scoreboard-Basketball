using AngularApp1.Server.Application.DTO.Game;
using AngularApp1.Server.Application.DTO.Score;

namespace AngularApp1.Server.Application.Interfaces
{
    public interface IGameService
    {
        Task<GameDto> CreateAsync(CreateGameDto dto);
        Task<GameDto?> GetAsync(long id);
        Task<GameDto> UpdateScoreAsync(long gameId, UpdateScoreDto dto);
        Task<GameDto> UploadTeamLogoAsync(long gameId, long teamId, IFormFile file);
        Task<GameDto> StartTimerAsync(long gameId);
        Task<GameDto> PauseTimerAsync(long gameId);
        Task<GameDto> ResetTimerAsync(long gameId, int? newQuarterSeconds = null);
        Task<GameDto> NextQuarterAsync(long gameId);
        Task<GameDto> PrevQuarterAsync(long gameId);
        Task<GameDto> FinishAsync(long gameId);
        Task<GameDto> UpdateFoulsAsync(long gameId, long teamId, int delta);
        Task<IReadOnlyList<GameListItemDto>> GetAllAsync();
    }
}

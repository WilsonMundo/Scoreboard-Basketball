using AngularApp1.Server.Application.DTO.Game;
using static AngularApp1.Server.Application.DTO.Matches.Match;

namespace AngularApp1.Server.Application.Interfaces
{
    public interface IAdminMatchService
    {
        Task<ResultAPI<GameDto>> CreateFromCatalogAsync(CreateMatchDto dto);
        Task<ResultAPI<GameDto>> AssignRosterAsync(long gameId, AssignRosterDto dto);
        Task<ResultAPI<IReadOnlyList<MatchHistoryItemDto>>> GetHistoryAsync();
    }

}

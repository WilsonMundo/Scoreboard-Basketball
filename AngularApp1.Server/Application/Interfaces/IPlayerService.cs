using AngularApp1.Server.Application.DTO.Players;
using AngularApp1.Server.Application.DTO.Teams;

namespace AngularApp1.Server.Application.Interfaces
{
    public interface IPlayerService
    {
        Task<ResultAPI<PlayerDto>> CreateAsync(PlayerCreateDto dto);
        Task<ResultAPI<PlayerDto>> UpdateAsync(long id, PlayerUpdateDto dto);
        Task<ResultAPI<object>> DeleteAsync(long id);
        Task<ResultAPI<PlayerDto>> GetByIdAsync(long id);
        Task<ResultAPI<PagedResult<PlayerDto>>> ListAsync(PlayerListRequest req);
    }
}

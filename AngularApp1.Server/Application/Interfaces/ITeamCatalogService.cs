using AngularApp1.Server.Application.DTO.Teams;

namespace AngularApp1.Server.Application.Interfaces
{
    public interface ITeamCatalogService
    {
        Task<ResultAPI<TeamDto>> CreateAsync(TeamCreateDto dto);
        Task<ResultAPI<TeamDto>> UpdateAsync(long id, TeamUpdateDto dto);
        Task<ResultAPI<object>> DeleteAsync(long id);
        Task<ResultAPI<TeamDto>> GetByIdAsync(long id);
        Task<ResultAPI<PagedResult<TeamDto>>> ListAsync(TeamListRequest req);
        Task<ResultAPI<TeamDto>> UploadLogoAsync(long id, IFormFile file);
    }
}

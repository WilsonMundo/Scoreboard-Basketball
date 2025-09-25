using AngularApp1.Server.Domain.Model;

namespace AngularApp1.Server.Domain.Interface
{
    public interface ITeamCatalogRepository
    {
        Task<TeamCatalog?> GetByIdAsync(long id);
        Task<bool> ExistsByNameAsync(string name, long? excludeId = null);
        Task AddAsync(TeamCatalog entity);
        Task UpdateAsync(TeamCatalog entity);
        Task DeleteAsync(TeamCatalog entity);
        Task<(IReadOnlyList<TeamCatalog> Items, int Total)> ListAsync(string? q, int page, int size, string? sort);
        Task SaveChangesAsync();
    }
}

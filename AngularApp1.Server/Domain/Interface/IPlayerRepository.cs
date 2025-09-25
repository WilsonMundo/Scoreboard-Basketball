using AngularApp1.Server.Domain.Model;

namespace AngularApp1.Server.Domain.Interface
{
    public interface IPlayerRepository
    {
        Task<Player?> GetByIdAsync(long id);
        Task AddAsync(Player entity);
        Task UpdateAsync(Player entity);
        Task DeleteAsync(Player entity);
        Task<(IReadOnlyList<Player> Items, int Total)> ListAsync(string? q, long? teamId, int page, int size, string? sort);
        Task<bool> ExistsSameNumberInTeamAsync(long teamId, int number, long? excludeId = null);
        Task SaveChangesAsync();
    }
}

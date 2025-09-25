using AngularApp1.Server.Domain.Model;

namespace AngularApp1.Server.Domain.Interface
{
    public interface IGameAdminRepository
    {
        // Game
        Task AddGameAsync(Game game);
        Task<Game?> GetGameAsync(long id);
        Task<IReadOnlyList<Game>> GetGamesAsync();


        // GameTeam
        Task AddGameTeamAsync(GameTeam gt);
        Task<bool> ExistsGameTeamAsync(long gameId, long teamId);


        // GameRoster
        Task AddRosterRangeAsync(IEnumerable<GameRoster> roster);
        Task<IReadOnlyList<GameRoster>> GetRosterAsync(long gameId, long teamId);
        Task DeleteRosterAsync(IEnumerable<GameRoster> roster);


        Task SaveChangesAsync();
    }
}

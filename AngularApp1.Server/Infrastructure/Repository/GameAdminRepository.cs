using AngularApp1.Server.Domain.Interface;
using AngularApp1.Server.Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace AngularApp1.Server.Infrastructure.Repository
{
    public class GameAdminRepository : IGameAdminRepository
    {
        private readonly AppDbContext _db;
        public GameAdminRepository(AppDbContext db) { _db = db; }


        public async Task AddGameAsync(Game game) => await _db.Games.AddAsync(game);
        public async Task<Game?> GetGameAsync(long id) =>
        await _db.Games
        .Include(g => g.Teams)
        .Include(g => g.QuarterScores)
        .FirstOrDefaultAsync(g => g.Id == id);
        public async Task<IReadOnlyList<Game>> GetGamesAsync() =>
        await _db.Games.Include(g => g.Teams).AsNoTracking().OrderByDescending(g => g.CreatedAtUtc).ToListAsync();


        public async Task AddGameTeamAsync(GameTeam gt) => await _db.GameTeams.AddAsync(gt);
        public async Task<bool> ExistsGameTeamAsync(long gameId, long teamId) =>
        await _db.GameTeams.AnyAsync(x => x.GameId == gameId && x.TeamId == teamId);


        public async Task AddRosterRangeAsync(IEnumerable<GameRoster> roster) => await _db.GameRosters.AddRangeAsync(roster);
        public async Task<IReadOnlyList<GameRoster>> GetRosterAsync(long gameId, long teamId) =>
        await _db.GameRosters.Where(r => r.GameId == gameId && r.TeamId == teamId).AsNoTracking().ToListAsync();
        public async Task DeleteRosterAsync(IEnumerable<GameRoster> roster)
        {
            _db.GameRosters.RemoveRange(roster);
            await Task.CompletedTask;
        }


        public async Task SaveChangesAsync() => await _db.SaveChangesAsync();
    }
}

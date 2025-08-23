using AngularApp1.Server.Domain.Interface;
using AngularApp1.Server.Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace AngularApp1.Server.Infrastructure.Repository
{
    public class GameRepository : IGameRepository
    {
        private readonly AppDbContext _db;
        public GameRepository(AppDbContext db) => _db = db;

        public Task<Game?> GetAsync(long id) =>
            _db.Games.Include(g => g.Teams)
            .Include(g => g.QuarterScores)
            .FirstOrDefaultAsync(g => g.Id == id);
        public async Task AddAsync(Game game) => await _db.Games.AddAsync(game);
        public Task SaveChangesAsync() => _db.SaveChangesAsync();
    }
}

using AngularApp1.Server.Domain.Interface;
using AngularApp1.Server.Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace AngularApp1.Server.Infrastructure.Repository
{
    public class PlayerRepository : IPlayerRepository
    {
        private readonly AppDbContext _db;
        public PlayerRepository(AppDbContext db) { _db = db; }


        public async Task<Player?> GetByIdAsync(long id)
        => await _db.Players.Include(p => p.Team).FirstOrDefaultAsync(x => x.Id == id);


        public async Task AddAsync(Player entity) => await _db.Players.AddAsync(entity);


        public Task UpdateAsync(Player entity) { _db.Players.Update(entity); return Task.CompletedTask; }


        public Task DeleteAsync(Player entity) { _db.Players.Remove(entity); return Task.CompletedTask; }


        public async Task<(IReadOnlyList<Player> Items, int Total)> ListAsync(string? q, long? teamId, int page, int size, string? sort)
        {
            page = Math.Max(1, page);
            size = Math.Clamp(size, 1, 100);
            IQueryable<Player> query = _db.Players.Include(p => p.Team).AsNoTracking();


            if (!string.IsNullOrWhiteSpace(q))
            {
                var like = q.Trim();
                query = query.Where(p => p.FullName.Contains(like) || (p.Nationality != null && p.Nationality.Contains(like)));
            }
            if (teamId.HasValue)
                query = query.Where(p => p.TeamId == teamId.Value);


            query = sort switch
            {
                "-name" => query.OrderByDescending(x => x.FullName),
                "number" => query.OrderBy(x => x.Number),
                "-number" => query.OrderByDescending(x => x.Number),
                "age" => query.OrderBy(x => x.Age),
                "-age" => query.OrderByDescending(x => x.Age),
                "created" => query.OrderBy(x => x.CreatedAtUtc),
                "-created" => query.OrderByDescending(x => x.CreatedAtUtc),
                _ => query.OrderBy(x => x.FullName)
            };


            var total = await query.CountAsync();
            var items = await query.Skip((page - 1) * size).Take(size).ToListAsync();
            return (items, total);
        }


        public async Task<bool> ExistsSameNumberInTeamAsync(long teamId, int number, long? excludeId = null)
        => await _db.Players.AnyAsync(p => p.TeamId == teamId && p.Number == number && (!excludeId.HasValue || p.Id != excludeId.Value));


        public async Task SaveChangesAsync() => await _db.SaveChangesAsync();
    }
}

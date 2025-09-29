using AngularApp1.Server.Domain.Interface;
using AngularApp1.Server.Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace AngularApp1.Server.Infrastructure.Repository
{
    public class TeamCatalogRepository : ITeamCatalogRepository
    {
        private readonly AppDbContext _db;
        public TeamCatalogRepository(AppDbContext db) { _db = db; }


        public async Task<TeamCatalog?> GetByIdAsync(long id)
        => await _db.TeamCatalogs.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);


        public async Task<bool> ExistsByNameAsync(string name, long? excludeId = null)
        => await _db.TeamCatalogs.AnyAsync(x => x.Name == name && (!excludeId.HasValue || x.Id != excludeId.Value));


        public async Task AddAsync(TeamCatalog entity)
        {
            await _db.TeamCatalogs.AddAsync(entity);
        }


        public Task UpdateAsync(TeamCatalog entity)
        {
            _db.TeamCatalogs.Update(entity);
            return Task.CompletedTask;
        }


        public Task DeleteAsync(TeamCatalog entity)
        {
            _db.TeamCatalogs.Remove(entity);
            return Task.CompletedTask;
        }


        public async Task<(IReadOnlyList<TeamCatalog> Items, int Total)> ListAsync(string? q, int page, int size, string? sort, string? city = null)
        {
            page = Math.Max(1, page);
            size = Math.Clamp(size, 1, 100);


            IQueryable<TeamCatalog> query = _db.TeamCatalogs.AsNoTracking();


            if (!string.IsNullOrWhiteSpace(q))
            {
                var like = q.Trim();
                query = query.Where(t => t.Name.Contains(like) || (t.City != null && t.City.Contains(like)));
            }

            if (!string.IsNullOrWhiteSpace(city))
            {
                query = query.Where(t => t.City == city);
            }


            query = sort switch
            {
                "-name" => query.OrderByDescending(x => x.Name),
                "created" => query.OrderBy(x => x.CreatedAtUtc),
                "-created" => query.OrderByDescending(x => x.CreatedAtUtc),
                _ => query.OrderBy(x => x.Name)
            };


            var total = await query.CountAsync();
            var items = await query.Skip((page - 1) * size).Take(size).ToListAsync();
            return (items, total);
        }


        public async Task SaveChangesAsync() => await _db.SaveChangesAsync();
    }
}

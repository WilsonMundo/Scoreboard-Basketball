using AngularApp1.Server.Domain.Model;

namespace AngularApp1.Server.Domain.Interface
{


    public interface IGameRepository
    {
        public Task<Game?> GetAsync(long id);
        Task AddAsync(Game game);
        Task SaveChangesAsync();
    }
}

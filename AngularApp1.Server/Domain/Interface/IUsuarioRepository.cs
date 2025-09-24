using AngularApp1.Server.Domain.ModelRegister;

namespace AngularApp1.Server.Domain.Interface
{
    public interface IUsuarioRepository
    {
        void add(Usuario usuario);
        Task AddAsync(Usuario usuario);
        Task Edit(Usuario usuario);
        Task<bool> ExistsUsuario(string email);
        Task<Usuario?> GetUsuarioAsync(string email);

    }
}

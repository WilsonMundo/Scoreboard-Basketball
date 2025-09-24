using AngularApp1.Server.Application.DTO.Auth;

namespace AngularApp1.Server.Application.Interfaces
{
    public interface IAuthService
    {
        Task<ResultAPI<object>> RegisterUser(UserRegister user);
        Task<ResultAPI<UserToken?>> ValidateUser(UserInfo userDTO);
    }
}

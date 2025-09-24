namespace AngularApp1.Server.Application.DTO.Auth
{
    public class UserToken
    {
        public string Token { get; set; } = null!;
        public DateTime Expiration { get; set; }
    }
}

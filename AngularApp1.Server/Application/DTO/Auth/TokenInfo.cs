namespace AngularApp1.Server.Application.DTO.Auth
{
    public class TokenInfo
    {
        public string email { get; set; } = null!;
        public long idUser { get; set; }
        public string name { get; set; } = null!;
        public string? rolAdmin { get; set; }
    }
}

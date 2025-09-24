namespace AngularApp1.Server.Application.DTO.Auth
{
    public class UserInfoModel
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public IEnumerable<ClaimModel>? Claims { get; set; }
        public class ClaimModel
        {
            public string Type { get; set; } = null!;
            public string Value { get; set; } = null!;
        }
    }
}

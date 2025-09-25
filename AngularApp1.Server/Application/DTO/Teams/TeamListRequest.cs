namespace AngularApp1.Server.Application.DTO.Teams
{
    public class TeamListRequest
    {
        public string? Q { get; set; }
        public int Page { get; set; } = 1; // 1-based
        public int Size { get; set; } = 20; // max 100
        public string? Sort { get; set; } = "name";
    }

    public class PagedResult<T>
    {
        public int Page { get; set; }
        public int Size { get; set; }
        public int Total { get; set; }
        public IReadOnlyList<T> Items { get; set; } = Array.Empty<T>();
    }

}

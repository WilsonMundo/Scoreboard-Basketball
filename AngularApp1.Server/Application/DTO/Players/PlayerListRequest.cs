namespace AngularApp1.Server.Application.DTO.Players
{
    public class PlayerListRequest
    {
        public string? Q { get; set; }           // búsqueda por nombre/nacionalidad
        public long? TeamId { get; set; }       // filtro por equipo
        public int Page { get; set; } = 1;      // paginación (1-based)
        public int Size { get; set; } = 20;     // tamaño de página
        public string? Sort { get; set; } = "name"; 
        // opciones: name|-name|created|-created|number|-number|age|-age
    }
}

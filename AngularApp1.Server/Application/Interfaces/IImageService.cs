namespace AngularApp1.Server.Application.Interfaces
{
    public interface IImageService
    {
        /// <summary>
        /// Procesa (valida, redimensiona y comprime) y guarda la imagen en wwwroot/{subfolder}.
        /// Devuelve la URL relativa (ej. /logos/xyz.jpg)
        /// </summary>
        Task<string> SaveCompressedAsync(
            IFormFile file,
            string subfolder,
            string fileNameBase,
            int maxWidth = 300,
            int maxHeight = 300,
            int jpegQuality = 80
        );
    }
}

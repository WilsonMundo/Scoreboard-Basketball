using AngularApp1.Server.Application.Interfaces;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace AngularApp1.Server.Application.Services
{
    public class ImageService : IImageService
    {
        private static readonly HashSet<string> _allowed = new(StringComparer.OrdinalIgnoreCase)
        { ".png", ".jpg", ".jpeg", ".gif"  };

        public async Task<string> SaveCompressedAsync(
            IFormFile file,
            string subfolder,
            string fileNameBase,
            int maxWidth = 300,
            int maxHeight = 300,
            int jpegQuality = 80)
        {
            if (file is null || file.Length == 0)
                throw new ArgumentException("Archivo vacío");

            var ext = Path.GetExtension(file.FileName);
            if (!_allowed.Contains(ext))
                throw new ArgumentException("Formato no permitido (usa .png, .jpg, .jpeg, .gif)");

            // Ruta física: wwwroot/{subfolder}
            var root = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", subfolder);
            Directory.CreateDirectory(root);

            // Construye nombre final (siempre generamos un nuevo nombre)
            var safeBase = fileNameBase.Replace(" ", "_");
            var finalName = $"{safeBase}_{Guid.NewGuid():N}{ext.ToLowerInvariant()}";
            var finalPath = Path.Combine(root, finalName);

            using var input = file.OpenReadStream();
            using var image = await Image.LoadAsync(input);

            // Calcula tamaño objetivo manteniendo aspect ratio (fit dentro de maxW x maxH)
            var (nw, nh) = GetFitSize(image.Width, image.Height, maxWidth, maxHeight);
            image.Mutate(x => x.Resize(new ResizeOptions
            {
                Mode = ResizeMode.Max,
                Size = new Size(nw, nh),
                Sampler = KnownResamplers.Bicubic
            }));

            // Elegimos encoder: JPG si no hay alpha, si hay alpha -> PNG
            IImageEncoder encoder;
            bool hasAlpha = image.Metadata?.GetGifMetadata() is not null || image.PixelType.AlphaRepresentation != SixLabors.ImageSharp.PixelFormats.PixelAlphaRepresentation.None;

            if (!hasAlpha && (ext.Equals(".jpg", StringComparison.OrdinalIgnoreCase) || ext.Equals(".jpeg", StringComparison.OrdinalIgnoreCase)))
            {
                encoder = new JpegEncoder { Quality = jpegQuality };
            }
            else if (!hasAlpha)
            {
                // Aunque venga PNG, si no hay alpha, podemos recomprimir a JPG para menor peso
                finalName = Path.ChangeExtension(finalName, ".jpg");
                finalPath = Path.ChangeExtension(finalPath, ".jpg");
                encoder = new JpegEncoder { Quality = jpegQuality };
            }
            else
            {
                // Mantener PNG para transparencia (menor pérdida visual)
                encoder = new PngEncoder { CompressionLevel = PngCompressionLevel.Level6 };
                // Si venía jpg pero tiene alpha (raro), se cambia  png
                if (ext.Equals(".jpg", StringComparison.OrdinalIgnoreCase) || ext.Equals(".jpeg", StringComparison.OrdinalIgnoreCase))
                {
                    finalName = Path.ChangeExtension(finalName, ".png");
                    finalPath = Path.ChangeExtension(finalPath, ".png");
                }
            }

            // Tamaño de salida fijo (p. ej. 512x384 = 4:3 como tu caja 128x96)
            var targetW = maxWidth;
            var targetH = maxHeight;

            // 1) Resize para que quepa dentro sin recortar (como contain)
            var (rw, rh) = GetFitSize(image.Width, image.Height, targetW, targetH);
            image.Mutate(x => x.Resize(new ResizeOptions
            {
                Mode = ResizeMode.Max,           // ajusta dentro
                Size = new Size(rw, rh),
                Sampler = KnownResamplers.Bicubic
            }));

            // 2) Crear lienzo final fijo y centrar (letterbox/pillarbox)
            using var canvas = new Image<Rgba32>(targetW, targetH, new Rgba32(248, 250, 252, 255)); // bg-slate-50 aprox
            canvas.Mutate(x => x.DrawImage(image, new Point((targetW - rw) / 2, (targetH - rh) / 2), 1f));

            await image.SaveAsync(finalPath, encoder);

            // URL pública relativa
            return $"/{subfolder}/{finalName}";
        }

        private static (int w, int h) GetFitSize(int srcW, int srcH, int maxW, int maxH)
        {
            if (srcW <= maxW && srcH <= maxH) return (srcW, srcH);
            var ratio = Math.Min((double)maxW / srcW, (double)maxH / srcH);
            return ((int)Math.Round(srcW * ratio), (int)Math.Round(srcH * ratio));
        }
    }
}

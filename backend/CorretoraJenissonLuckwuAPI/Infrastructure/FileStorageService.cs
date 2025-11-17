using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace CorretoraJenissonLuckwuAPI.Infrastructure
{
    public class FileStorageService
    {
        #region Variables
        private readonly IWebHostEnvironment _environment;
        private readonly string _webRootPath;
        private readonly string _imoveisRelativePath;
        private readonly long _maxFileSizeBytes;
        private static readonly string[] _allowedExtensions = [".jpg", ".jpeg", ".png", ".webp"];
        #endregion

        #region Constructors
        public FileStorageService(IWebHostEnvironment environment, IConfiguration configuration)
        {
            _environment = environment;
            _webRootPath = environment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            _imoveisRelativePath = configuration["FileStorage:ImoveisPath"] ?? "uploads/imoveis";
            var maxFileSizeMb = configuration.GetValue<int?>("FileStorage:MaxFileSizeMb") ?? 5;
            _maxFileSizeBytes = maxFileSizeMb * 1024L * 1024L;
            EnsureBaseDirectoryExists();
        }
        #endregion

        #region Public Methods
        public async Task<string> SaveImovelImageAsync(int imovelId, IFormFile file, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(file);

            if (file.Length == 0)
                throw new InvalidOperationException("Arquivo enviado está vazio.");

            if (file.Length > _maxFileSizeBytes)
                throw new InvalidOperationException($"Arquivo excede o tamanho máximo permitido de {_maxFileSizeBytes / (1024 * 1024)} MB.");

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!_allowedExtensions.Contains(extension))
                throw new InvalidOperationException("Formato de arquivo não suportado. Utilize imagens JPG, JPEG, PNG ou WEBP.");

            var relativeDirectory = Path.Combine(_imoveisRelativePath, imovelId.ToString());
            var absoluteDirectory = Path.Combine(_webRootPath, relativeDirectory);
            if (!Directory.Exists(absoluteDirectory))
            {
                Directory.CreateDirectory(absoluteDirectory);
            }

            var fileName = $"{Guid.NewGuid()}{extension}";
            var relativePath = Path.Combine(relativeDirectory, fileName);
            var absolutePath = Path.Combine(_webRootPath, relativePath);

            using (var stream = new FileStream(absolutePath, FileMode.Create))
            {
                await file.CopyToAsync(stream, cancellationToken);
            }

            return "/" + relativePath.Replace("\\", "/");
        }

        public Task DeleteImageAsync(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return Task.CompletedTask;

            var relativePath = GetRelativePath(url);
            if (string.IsNullOrWhiteSpace(relativePath))
                return Task.CompletedTask;

            var normalizedRelative = relativePath.TrimStart('/', '\\');
            var absolutePath = Path.Combine(_webRootPath, normalizedRelative.Replace("/", Path.DirectorySeparatorChar.ToString()));
            if (File.Exists(absolutePath))
            {
                File.Delete(absolutePath);
            }

            return Task.CompletedTask;
        }

        public string BuildPublicUrl(string relativePath, HttpRequest request)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
                return string.Empty;

            var baseUrl = $"{request.Scheme}://{request.Host}";
            if (!relativePath.StartsWith("/"))
            {
                relativePath = "/" + relativePath;
            }

            return $"{baseUrl}{relativePath}";
        }
        #endregion

        #region Private Methods
        private void EnsureBaseDirectoryExists()
        {
            var absoluteBase = Path.Combine(_webRootPath, _imoveisRelativePath);
            if (!Directory.Exists(absoluteBase))
            {
                Directory.CreateDirectory(absoluteBase);
            }
        }

        private static string GetRelativePath(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return null;

            if (Uri.TryCreate(url, UriKind.Absolute, out var absoluteUri))
            {
                return absoluteUri.AbsolutePath;
            }

            return url;
        }
        #endregion
    }
}


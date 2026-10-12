using DocumentService.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentService.Infrastructure.Storage
{
    public class LocalFileStorage : IFileStorage
    {
        private readonly string _rootPath;

        public LocalFileStorage(IConfiguration config)
        {
            _rootPath = config["Storage:RootPath"]
                        ?? Path.Combine(Directory.GetCurrentDirectory(), "uploads");

            Directory.CreateDirectory(_rootPath);
        }

        public async Task<string> SaveAsync(Stream stream, string fileName, CancellationToken ct)
        {
            // Organize by year/month to avoid giant folders
            var now = DateTime.UtcNow;
            var relativeDir = Path.Combine(now.Year.ToString(), now.Month.ToString("D2"));
            var absoluteDir = Path.Combine(_rootPath, relativeDir);
            Directory.CreateDirectory(absoluteDir);

            // Use GUID for filename to avoid collisions & path injection
            var newFileName = $"{Guid.NewGuid()}.pdf";
            var relativePath = Path.Combine(relativeDir, newFileName);
            var absolutePath = Path.Combine(_rootPath, relativePath);

            using var fileStream = new FileStream(absolutePath, FileMode.Create);
            await stream.CopyToAsync(fileStream, ct);

            // Return relative path (portable across machines)
            return relativePath.Replace("\\", "/");
        }

        public Task<Stream> GetAsync(string storagePath, CancellationToken ct)
        {
            var absolutePath = Path.Combine(_rootPath, storagePath);
            if (!File.Exists(absolutePath))
                throw new FileNotFoundException($"File not found: {storagePath}");

            Stream stream = new FileStream(absolutePath, FileMode.Open, FileAccess.Read);
            return Task.FromResult(stream);
        }

        public Task DeleteAsync(string storagePath, CancellationToken ct)
        {
            var absolutePath = Path.Combine(_rootPath, storagePath);
            if (File.Exists(absolutePath))
                File.Delete(absolutePath);

            return Task.CompletedTask;
        }
    }
}

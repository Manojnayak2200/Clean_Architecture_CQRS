using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentService.Application.Interfaces
{
    public interface IFileStorage
    {
        Task<string> SaveAsync(Stream stream, string fileName, CancellationToken ct);
        Task<Stream> GetAsync(string storagePath, CancellationToken ct);
        Task DeleteAsync(string storagePath, CancellationToken ct);
    }
}

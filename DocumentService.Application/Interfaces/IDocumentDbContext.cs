using DocumentService.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace DocumentService.Application.Interfaces
{
    public interface IDocumentDbContext
    {
        DbSet<DocumentFile> DocumentFile { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}

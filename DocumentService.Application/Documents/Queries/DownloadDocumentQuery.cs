using DocumentService.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentService.Application.Documents.Queries
{

    public class DownloadDocumentQuery : IRequest<DownloadResult?>
    {
        public Guid DocumentId { get; set; }
        public Guid RequestedByUserId { get; set; }
        public bool IsAdmin { get; set; }
    }

    public class DownloadResult
    {
        public Stream Stream { get; set; } = Stream.Null;
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = "application/pdf";
    }

    public class DownloadDocumentQueryHandler : IRequestHandler<DownloadDocumentQuery, DownloadResult?>
    {
        private readonly IDocumentDbContext _context;
        private readonly IFileStorage _storage;

        public DownloadDocumentQueryHandler(IDocumentDbContext context, IFileStorage storage)
        {
            _context = context;
            _storage = storage;
        }

        public async Task<DownloadResult?> Handle(DownloadDocumentQuery request, CancellationToken ct)
        {
            var doc = await _context.DocumentFile
                .FirstOrDefaultAsync(d => d.Id == request.DocumentId, ct);

            if (doc is null) return null;

            // Only owner or Admin can download
            if (!request.IsAdmin && doc.UploadedByUserId != request.RequestedByUserId)
                throw new UnauthorizedAccessException("You are not allowed to access this document.");

            var stream = await _storage.GetAsync(doc.StoragePath, ct);

            return new DownloadResult
            {
                Stream = stream,
                FileName = doc.FileName,
                ContentType = doc.ContentType
            };
        }
    }
}

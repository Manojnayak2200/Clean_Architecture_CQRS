using DocumentService.Application.Interfaces;
using DocumentService.Domain;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace DocumentService.Application.Documents.Commands
{
    public class UploadDocumentCommand : IRequest<Guid>
    {
        public Stream FileStream { get; set; } = Stream.Null;
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public long SizeInBytes { get; set; }
        public Guid UploadedByUserId { get; set; }
        public Guid? EmployeeId { get; set; }
        public string? Description { get; set; }
    }

    public class UploadDocumentCommandHandler : IRequestHandler<UploadDocumentCommand, Guid>
    {
        private readonly IDocumentDbContext _context;
        private readonly IFileStorage _storage;

        public UploadDocumentCommandHandler(IDocumentDbContext context, IFileStorage storage)
        {
            _context = context;
            _storage = storage;
        }

        public async Task<Guid> Handle(UploadDocumentCommand request, CancellationToken ct)
        {
            // Validate: must be PDF
            if (!string.Equals(Path.GetExtension(request.FileName), ".pdf", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Only PDF files are allowed.");

            // Validate: max 5 MB
            const long maxSize = 5 * 1024 * 1024;
            if (request.SizeInBytes > maxSize)
                throw new InvalidOperationException("File size exceeds 5 MB limit.");

            // Save file to disk
            var storagePath = await _storage.SaveAsync(request.FileStream, request.FileName, ct);

            // Persist metadata
            var doc = new DocumentFile        // ✅ Changed from "Document" to "DocumentFile"
            {
                Id = Guid.NewGuid(),
                FileName = request.FileName,
                ContentType = "application/pdf",
                SizeInBytes = request.SizeInBytes,
                StoragePath = storagePath,
                UploadedByUserId = request.UploadedByUserId,
                EmployeeId = request.EmployeeId,
                Description = request.Description,
                UploadedAt = DateTime.UtcNow
            };


            _context.DocumentFile.Add(doc);
            await _context.SaveChangesAsync(ct);

            return doc.Id;
        }
    }
}

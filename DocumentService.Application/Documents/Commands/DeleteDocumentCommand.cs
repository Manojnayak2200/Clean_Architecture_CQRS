using DocumentService.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentService.Application.Documents.Commands
{
    public class DeleteDocumentCommand : IRequest<bool>
    {
        public Guid DocumentId { get; set; }
        public Guid RequestedByUserId { get; set; }
        public bool IsAdmin { get; set; }
    }

    public class DeleteDocumentCommandHandler : IRequestHandler<DeleteDocumentCommand, bool>
    {
        private readonly IDocumentDbContext _context;
        private readonly IFileStorage _storage;

        public DeleteDocumentCommandHandler(IDocumentDbContext context, IFileStorage storage)
        {
            _context = context;
            _storage = storage;
        }

        public async Task<bool> Handle(DeleteDocumentCommand request, CancellationToken ct)
        {
            var doc = await _context.DocumentFile
                .FirstOrDefaultAsync(d => d.Id == request.DocumentId, ct);

            if (doc is null) return false;

            // Only owner or Admin can delete
            if (!request.IsAdmin && doc.UploadedByUserId != request.RequestedByUserId)
                throw new UnauthorizedAccessException("You are not allowed to delete this document.");

            await _storage.DeleteAsync(doc.StoragePath, ct);

            _context.DocumentFile.Remove(doc);
            await _context.SaveChangesAsync(ct);

            return true;
        }
    }
}

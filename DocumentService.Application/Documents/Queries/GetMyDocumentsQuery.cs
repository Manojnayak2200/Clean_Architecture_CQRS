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
    public class GetMyDocumentsQuery : IRequest<List<DocumentResponse>>
    {
        public Guid UserId { get; set; }
    }

    public class GetMyDocumentsQueryHandler : IRequestHandler<GetMyDocumentsQuery, List<DocumentResponse>>
    {
        private readonly IDocumentDbContext _context;

        public GetMyDocumentsQueryHandler(IDocumentDbContext context) => _context = context;

        public async Task<List<DocumentResponse>> Handle(GetMyDocumentsQuery request, CancellationToken ct)
        {
            return await _context.DocumentFile
                .Where(d => d.UploadedByUserId == request.UserId)
                .OrderByDescending(d => d.UploadedAt)
                .Select(d => new DocumentResponse
                {
                    Id = d.Id,
                    FileName = d.FileName,
                    ContentType = d.ContentType,
                    SizeInBytes = d.SizeInBytes,
                    UploadedByUserId = d.UploadedByUserId,
                    EmployeeId = d.EmployeeId,
                    Description = d.Description,
                    UploadedAt = d.UploadedAt
                })
                .ToListAsync(ct);
        }
    }
}

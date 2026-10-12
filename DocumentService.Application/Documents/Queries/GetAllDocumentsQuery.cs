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
    public class GetAllDocumentsQuery : IRequest<List<DocumentResponse>> { }

    public class GetAllDocumentsQueryHandler : IRequestHandler<GetAllDocumentsQuery, List<DocumentResponse>>
    {
        private readonly IDocumentDbContext _context;

        public GetAllDocumentsQueryHandler(IDocumentDbContext context) => _context = context;

        public async Task<List<DocumentResponse>> Handle(GetAllDocumentsQuery request, CancellationToken ct)
        {
            return await _context.DocumentFile
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

using EmployeeService.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EmployeeService.Application.Interfaces.IEmployeeDbContext;

namespace EmployeeService.Application.Employees.Queries
{
    public class GetMyProfileWithDocsQuery : IRequest<object>
    {
        public Guid AuthUserId { get; set; }
    }

    public class GetMyProfileWithDocsQueryHandler
        : IRequestHandler<GetMyProfileWithDocsQuery, object>
    {
        private readonly IEmployeeDbContext _context;
        private readonly IDocumentServiceClient _documentClient;

        public GetMyProfileWithDocsQueryHandler(
            IEmployeeDbContext context,
            IDocumentServiceClient documentClient)
        {
            _context = context;
            _documentClient = documentClient;
        }

        public async Task<object> Handle(
            GetMyProfileWithDocsQuery request,
            CancellationToken ct)
        {
            var employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.AuthUserId == request.AuthUserId, ct);

            if (employee == null) return null!;

            //var documents = await _documentClient
            //    .GetDocumentsByUserIdAsync(employee.AuthUserId!.Value);

            return new
            {
                Employee = employee,
                Documents = "documents"
            };
        }
    }
}

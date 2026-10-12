using EmployeeService.Application.Interfaces;
using EmployeeService.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeService.Application.Employees.Queries
{
    public class GetMyProfileQuery : IRequest<Employee?>
    {
        public Guid AuthUserId { get; set; }
    }

    public class GetMyProfileQueryHandler
        : IRequestHandler<GetMyProfileQuery, Employee?>
    {
        private readonly IEmployeeDbContext _context;

        public GetMyProfileQueryHandler(IEmployeeDbContext context)
        {
            _context = context;
        }

        public async Task<Employee?> Handle(
            GetMyProfileQuery request,
            CancellationToken cancellationToken)
        {
            return await _context.Employees
                .FirstOrDefaultAsync(e => e.AuthUserId == request.AuthUserId,
                                    cancellationToken);
        }
    }
}

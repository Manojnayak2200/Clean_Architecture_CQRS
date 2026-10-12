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
    public class GetEmployeeByIdQuery : IRequest<Employee?>
    {
        public Guid Id { get; set; }
    }

    public class GetEmployeeByIdQueryHandler : IRequestHandler<GetEmployeeByIdQuery, Employee?>
    {
        private readonly IEmployeeDbContext _context;

        public GetEmployeeByIdQueryHandler(IEmployeeDbContext context)
        {
            _context = context;
        }

        public async Task<Employee?> Handle(GetEmployeeByIdQuery request, CancellationToken cancellationToken)
        {
            return await _context.Employees
                .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);
        }
    }
}

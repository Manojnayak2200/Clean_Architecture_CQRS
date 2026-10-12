using EmployeeService.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeService.Application.Employees.Commands
{

    public class LinkEmployeeToUserCommand : IRequest<bool>
    {
        public Guid EmployeeId { get; set; }
        public Guid AuthUserId { get; set; }
    }

    public class LinkEmployeeToUserCommandHandler
        : IRequestHandler<LinkEmployeeToUserCommand, bool>
    {
        private readonly IEmployeeDbContext _context;

        public LinkEmployeeToUserCommandHandler(IEmployeeDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(
            LinkEmployeeToUserCommand request,
            CancellationToken cancellationToken)
        {
            var employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.Id == request.EmployeeId, cancellationToken);

            if (employee == null) return false;

            employee.AuthUserId = request.AuthUserId;
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}

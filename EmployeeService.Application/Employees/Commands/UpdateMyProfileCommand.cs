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
    public class UpdateMyProfileCommand : IRequest<bool>
    {
        public Guid AuthUserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

    public class UpdateMyProfileRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

    public class UpdateMyProfileCommandHandler
        : IRequestHandler<UpdateMyProfileCommand, bool>
    {
        private readonly IEmployeeDbContext _context;

        public UpdateMyProfileCommandHandler(IEmployeeDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(
            UpdateMyProfileCommand request,
            CancellationToken cancellationToken)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(request.Name))
                throw new InvalidOperationException("Name is required");

            if (string.IsNullOrWhiteSpace(request.Email) || !request.Email.Contains("@"))
                throw new InvalidOperationException("Valid email is required");

            var employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.AuthUserId == request.AuthUserId,
                                    cancellationToken);

            if (employee == null) return false;

            // Check email conflict
            var emailTaken = await _context.Employees
                .AnyAsync(e => e.Email == request.Email && e.Id != employee.Id,
                         cancellationToken);

            if (emailTaken)
                throw new InvalidOperationException("Email already in use");

            // Update ONLY safe fields
            employee.Name = request.Name;
            employee.Email = request.Email;

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}

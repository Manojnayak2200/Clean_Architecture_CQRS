using AuthService.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Application.Auth.Queries
{
   

    // 🟢 DTO for returning user data (never return the PasswordHash!)
    public class UserDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        
    }

    // 🟢 The Query
    public class GetAllUsersQuery : IRequest<List<UserDto>> { }

    // 🟢 The Handler
    public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, List<UserDto>>
    {
        private readonly IAuthDbContext _context;

        public GetAllUsersQueryHandler(IAuthDbContext context)
        {
            _context = context;
        }

        public async Task<List<UserDto>> Handle(
            GetAllUsersQuery request,
            CancellationToken ct)
        {
            return await _context.Users
                .OrderBy(u => u.Username)
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    Username = u.Username,
                    Role = u.Role
                   
                })
                .ToListAsync(ct);
        }
    }
}

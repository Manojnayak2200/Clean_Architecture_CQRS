using AuthService.Domain.Interfaces;
using BCrypt.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Application.Auth.Commands
{
    public class LoginCommand : IRequest<AuthResponse>
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponse>
    {
        private readonly IAuthDbContext _context;
        private readonly IJwtTokenGenerator _tokenGenerator;

        public LoginCommandHandler(IAuthDbContext context, IJwtTokenGenerator tokenGenerator)
        {
            _context = context;
            _tokenGenerator = tokenGenerator;
        }

        public async Task<AuthResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            // Find user
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == request.Username, cancellationToken);

            if (user == null)
                throw new UnauthorizedAccessException("Invalid credentials");

            // Verify password
            bool validPassword = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
            if (!validPassword)
                throw new UnauthorizedAccessException("Invalid credentials");

            // Generate JWT
            var token = _tokenGenerator.GenerateToken(user);

            return new AuthResponse
            {
                Token = token,
                Username = user.Username,
                Role = user.Role
            };
        }
    }
}

using AuthService.Application.Auth;
using AuthService.Application.Auth.Commands;
using AuthService.Application.Auth.NewFolder;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var cmd = new RegisterCommand
            {
                Username = request.Username,
                Password = request.Password,
                Role = request.Role
            };
            var id = await _mediator.Send(cmd);
            return Ok(new { UserId = id });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                var cmd = new LoginCommand
                {
                    Username = request.Username,
                    Password = request.Password
                };
                var result = await _mediator.Send(cmd);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }
    }
}

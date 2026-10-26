using EmployeeService.Application.Employees.Commands;
using EmployeeService.Application.Employees.Queries;
using EmployeeService.Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using static EmployeeService.Application.Interfaces.IEmployeeDbContext;

// ============================================================
//  EMPLOYEES CONTROLLER
//  URLs use method names for clarity
// ============================================================
namespace EmployeeService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EmployeesController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IDocumentServiceClient _context;
        public EmployeesController(IMediator mediator, IDocumentServiceClient context)
        {
            _mediator = mediator;
            _context = context;
        }

        private Guid GetCurrentUserId()
        {
            var sub = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                   ?? User.FindFirst("sub")?.Value;
            return Guid.TryParse(sub, out var id) ? id : Guid.Empty;
        }

        // ============================================================
        #region 👤 EMPLOYEE SELF-SERVICE
        // ============================================================

        // GET /api/employees/GetMyProfile
        [HttpGet("GetMyProfile")]
        public async Task<IActionResult> GetMyProfile()
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
                return Unauthorized(new { message = "Invalid token" });

            var profile = await _mediator.Send(
                new GetMyProfileQuery { AuthUserId = userId });

            return profile is null
                ? NotFound(new { message = "No employee profile linked to your account" })
                : Ok(profile);
        }

        // PUT /api/employees/UpdateMyProfile
        [HttpPut("UpdateMyProfile")]
        public async Task<IActionResult> UpdateMyProfile(
            [FromBody] UpdateMyProfileRequest request)
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
                return Unauthorized(new { message = "Invalid token" });

            try
            {
                var success = await _mediator.Send(new UpdateMyProfileCommand
                {
                    AuthUserId = userId,
                    Name = request.Name,
                    Email = request.Email
                });

                return success
                    ? Ok(new { message = "Profile updated successfully" })
                    : NotFound(new { message = "No employee profile linked to your account" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        #endregion

        // ============================================================
        #region 🔐 ADMIN ONLY
        // ============================================================

        // GET /api/employees/GetAllEmployees
        [HttpGet("GetAllEmployees")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _mediator.Send(new GetAllEmployeesQuery());
            return Ok(result);
        }

        // GET /api/employees/GetEmployeeById/{id}
        [HttpGet("GetEmployeeById/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetEmployeeByIdQuery { Id = id });
            return result is null ? NotFound() : Ok(result);
        }

        // POST /api/employees/CreateEmployee
        [HttpPost("CreateEmployee")]
        public async Task<IActionResult> Create([FromBody] CreateEmployeeCommand command)
        {
            var id = await _mediator.Send(command);
            return Ok(new { Id = id });
        }

        // POST /api/employees/LinkEmployeeToUser/{id}
        [HttpPost("LinkEmployeeToUser/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> LinkToUser(
            Guid id,
            [FromBody] LinkRequest request)
        {
            var success = await _mediator.Send(new LinkEmployeeToUserCommand
            {
                EmployeeId = id,
                AuthUserId = request.AuthUserId
            });

            return success
                ? Ok(new { message = "Employee linked successfully" })
                : NotFound(new { message = "Employee not found" });
        }

        // DELETE /api/employees/DeleteEmployee/{id}
        [HttpDelete("DeleteEmployee/{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(Guid id)
        {
            return Ok(new { Deleted = id });
        }

        #endregion


        [HttpGet("GetMyDOC")]
        [Authorize]
        public async Task<IActionResult> GetByUserId()
        {
            var docs = await _context.GetDocumentsByUserIdAsync();
              

            return Ok(docs);
        }
    }

    public class LinkRequest
    {
        public Guid AuthUserId { get; set; }
    }
}
using LeaveService.Application.Leaves.Commands;
using LeaveService.Application.Leaves.Queries;
using LeaveService.Domain;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LeaveService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class LeavesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public LeavesController(IMediator mediator) => _mediator = mediator;

        private Guid GetCurrentUserId()
        {
            var sub = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                   ?? User.FindFirst("sub")?.Value;
            return Guid.TryParse(sub, out var id) ? id : Guid.Empty;
        }

        private bool IsAdmin() => User.IsInRole("Admin");

        // ============================================================
        #region 👤 EMPLOYEE SELF-SERVICE
        // ============================================================

        [HttpPost("ApplyLeave")]
        public async Task<IActionResult> ApplyLeave([FromBody] ApplyLeaveRequest request)
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty) return Unauthorized();

            try
            {
                var id = await _mediator.Send(new ApplyLeaveCommand
                {
                    EmployeeId = userId,
                    RequestedByUserId = userId,
                    Type = request.Type,
                    StartDate = request.StartDate,
                    EndDate = request.EndDate,
                    Reason = request.Reason
                });
                return Ok(new { Id = id });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("MyLeaves")]
        public async Task<IActionResult> GetMyLeaves()
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty) return Unauthorized();

            var result = await _mediator.Send(new GetMyLeavesQuery { EmployeeId = userId });
            return Ok(result);
        }

        [HttpGet("MyBalance")]
        public async Task<IActionResult> GetMyBalance()
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty) return Unauthorized();

            var result = await _mediator.Send(new GetLeaveBalanceQuery { EmployeeId = userId });
            return Ok(result);
        }

        #endregion

        // ============================================================
        #region 🔐 ADMIN ONLY
        // ============================================================

        [HttpGet("PendingLeaves")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetPendingLeaves()
        {
            var result = await _mediator.Send(new GetPendingLeavesQuery());
            return Ok(result);
        }

        [HttpPost("ApproveLeave/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ApproveLeave(Guid id, [FromBody] AdminActionRequest request)
        {
            var adminId = GetCurrentUserId();
            if (adminId == Guid.Empty) return Unauthorized();

            try
            {
                var success = await _mediator.Send(new ApproveLeaveCommand
                {
                    LeaveRequestId = id,
                    ApprovedByUserId = adminId,
                    Comments = request.Comments
                });

                return success
                    ? Ok(new { message = "Leave approved." })
                    : NotFound(new { message = "Leave request not found." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("RejectLeave/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RejectLeave(Guid id, [FromBody] AdminActionRequest request)
        {
            var adminId = GetCurrentUserId();
            if (adminId == Guid.Empty) return Unauthorized();

            try
            {
                var success = await _mediator.Send(new RejectLeaveCommand
                {
                    LeaveRequestId = id,
                    ApprovedByUserId = adminId,
                    Comments = request.Comments
                });

                return success
                    ? Ok(new { message = "Leave rejected." })
                    : NotFound(new { message = "Leave request not found." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        #endregion
    }

    public class ApplyLeaveRequest
    {
        public LeaveType Type { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Reason { get; set; }
    }

    public class AdminActionRequest
    {
        public string? Comments { get; set; }
    }
}

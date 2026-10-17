using LeaveService.Application.Interfaces;
using LeaveService.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaveService.Application.Leaves.Commands
{
    public class RejectLeaveCommand : IRequest<bool>
    {
        public Guid LeaveRequestId { get; set; }
        public Guid ApprovedByUserId { get; set; }
        public string? Comments { get; set; }
    }

    public class RejectLeaveCommandHandler : IRequestHandler<RejectLeaveCommand, bool>
    {
        private readonly ILeaveDbContext _context;

        public RejectLeaveCommandHandler(ILeaveDbContext context) => _context = context;

        public async Task<bool> Handle(RejectLeaveCommand request, CancellationToken ct)
        {
            var leave = await _context.LeaveRequests
                .FirstOrDefaultAsync(l => l.Id == request.LeaveRequestId, ct);

            if (leave is null) return false;

            if (leave.Status != LeaveStatus.Pending)
                throw new InvalidOperationException("Only pending requests can be rejected.");

            leave.Status = LeaveStatus.Rejected;
            leave.ApprovedByUserId = request.ApprovedByUserId;
            leave.ApproverComments = request.Comments;
            leave.ActionedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(ct);
            return true;
        }
    }
}

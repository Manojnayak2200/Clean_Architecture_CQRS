using LeaveService.Application.Interfaces;
using LeaveService.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaveService.Application.Leaves.Queries
{
    public class GetPendingLeavesQuery : IRequest<List<LeaveRequestDto>> { }

    public class GetPendingLeavesQueryHandler : IRequestHandler<GetPendingLeavesQuery, List<LeaveRequestDto>>
    {
        private readonly ILeaveDbContext _context;

        public GetPendingLeavesQueryHandler(ILeaveDbContext context) => _context = context;

        public async Task<List<LeaveRequestDto>> Handle(GetPendingLeavesQuery request, CancellationToken ct)
        {
            return await _context.LeaveRequests
                .Where(l => l.Status == LeaveStatus.Pending)
                .OrderBy(l => l.RequestedAt)
                .Select(l => new LeaveRequestDto
                {
                    Id = l.Id,
                    EmployeeId = l.EmployeeId,
                    Type = l.Type,
                    StartDate = l.StartDate,
                    EndDate = l.EndDate,
                    TotalDays = l.TotalDays,
                    Reason = l.Reason,
                    Status = l.Status,
                    RequestedAt = l.RequestedAt,
                    ApproverComments = l.ApproverComments,
                    ActionedAt = l.ActionedAt
                })
                .ToListAsync(ct);
        }
    }
}

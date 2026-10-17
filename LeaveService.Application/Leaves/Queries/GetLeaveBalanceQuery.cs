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
    public class GetLeaveBalanceQuery : IRequest<LeaveBalanceDto>
    {
        public Guid EmployeeId { get; set; }
        public int Year { get; set; } = DateTime.UtcNow.Year;
    }

    public class GetLeaveBalanceQueryHandler : IRequestHandler<GetLeaveBalanceQuery, LeaveBalanceDto>
    {
        private readonly ILeaveDbContext _context;

        public GetLeaveBalanceQueryHandler(ILeaveDbContext context) => _context = context;

        public async Task<LeaveBalanceDto> Handle(GetLeaveBalanceQuery request, CancellationToken ct)
        {
            var approved = await _context.LeaveRequests
                .Where(l => l.EmployeeId == request.EmployeeId
                         && l.Status == LeaveStatus.Approved
                         && l.StartDate.Year == request.Year)
                .GroupBy(l => l.Type)
                .Select(g => new { Type = g.Key, Days = g.Sum(x => x.TotalDays) })
                .ToListAsync(ct);

            return new LeaveBalanceDto
            {
                Year = request.Year,
                SickUsed = approved.FirstOrDefault(a => a.Type == LeaveType.Sick)?.Days ?? 0,
                CasualUsed = approved.FirstOrDefault(a => a.Type == LeaveType.Casual)?.Days ?? 0,
                EarnedUsed = approved.FirstOrDefault(a => a.Type == LeaveType.Earned)?.Days ?? 0
            };
        }
    }
}

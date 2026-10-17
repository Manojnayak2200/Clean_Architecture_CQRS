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

    public class ApplyLeaveCommand : IRequest<Guid>
    {
        public Guid EmployeeId { get; set; }
        public Guid RequestedByUserId { get; set; }
        public LeaveType Type { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Reason { get; set; }
    }

    public class ApplyLeaveCommandHandler : IRequestHandler<ApplyLeaveCommand, Guid>
    {
        private readonly ILeaveDbContext _context;

        public ApplyLeaveCommandHandler(ILeaveDbContext context) => _context = context;

        public async Task<Guid> Handle(ApplyLeaveCommand request, CancellationToken ct)
        {
            // Basic business rules (skip if using FluentValidation later)
            if (request.StartDate > request.EndDate)
                throw new InvalidOperationException("Start date must be before end date.");

            var totalDays = (request.EndDate.Date - request.StartDate.Date).Days + 1;

            if (totalDays <= 0)
                throw new InvalidOperationException("Total days must be at least 1.");

            // Leave balance check
            var currentYear = request.StartDate.Year;
            var usedDays = await _context.LeaveRequests
                .Where(l => l.EmployeeId == request.EmployeeId
                         && l.Type == request.Type
                         && l.Status == LeaveStatus.Approved
                         && l.StartDate.Year == currentYear)
                .SumAsync(l => (int?)l.TotalDays, ct) ?? 0;

            var maxDays = request.Type switch
            {
                LeaveType.Sick => 12,
                LeaveType.Casual => 10,
                LeaveType.Earned => 15,
                _ => 0
            };

            if (usedDays + totalDays > maxDays)
                throw new InvalidOperationException(
                    $"Insufficient {request.Type} leave. Used {usedDays}, requested {totalDays}, max {maxDays}.");

            var leave = new LeaveRequest
            {
                Id = Guid.NewGuid(),
                EmployeeId = request.EmployeeId,
                Type = request.Type,
                StartDate = request.StartDate.Date,
                EndDate = request.EndDate.Date,
                TotalDays = totalDays,
                Reason = request.Reason,
                Status = LeaveStatus.Pending,
                RequestedByUserId = request.RequestedByUserId,
                RequestedAt = DateTime.UtcNow
            };

            _context.LeaveRequests.Add(leave);
            await _context.SaveChangesAsync(ct);
            return leave.Id;
        }
    }
}

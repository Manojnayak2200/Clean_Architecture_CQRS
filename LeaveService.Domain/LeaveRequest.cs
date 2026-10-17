using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaveService.Domain
{
    public enum LeaveType
    {
        Sick = 0,
        Casual = 1,
        Earned = 2
    }

    public enum LeaveStatus
    {
        Pending = 0,
        Approved = 1,
        Rejected = 2
    }

    public class LeaveRequest
    {
        public Guid Id { get; set; }
        public Guid EmployeeId { get; set; }
        public LeaveType Type { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TotalDays { get; set; }
        public string? Reason { get; set; }
        public LeaveStatus Status { get; set; } = LeaveStatus.Pending;
        public Guid RequestedByUserId { get; set; }
        public Guid? ApprovedByUserId { get; set; }
        public string? ApproverComments { get; set; }
        public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ActionedAt { get; set; }
    }
}

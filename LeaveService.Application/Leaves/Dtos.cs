using LeaveService.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaveService.Application.Leaves
{
    public class LeaveRequestDto
    {
        public Guid Id { get; set; }
        public Guid EmployeeId { get; set; }
        public LeaveType Type { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TotalDays { get; set; }
        public string? Reason { get; set; }
        public LeaveStatus Status { get; set; }
        public DateTime RequestedAt { get; set; }
        public string? ApproverComments { get; set; }
        public DateTime? ActionedAt { get; set; }
    }

    public class LeaveBalanceDto
    {
        public int Year { get; set; }
        public int SickTotal { get; set; } = 12;
        public int SickUsed { get; set; }
        public int SickRemaining => SickTotal - SickUsed;
        public int CasualTotal { get; set; } = 10;
        public int CasualUsed { get; set; }
        public int CasualRemaining => CasualTotal - CasualUsed;
        public int EarnedTotal { get; set; } = 15;
        public int EarnedUsed { get; set; }
        public int EarnedRemaining => EarnedTotal - EarnedUsed;
    }
}

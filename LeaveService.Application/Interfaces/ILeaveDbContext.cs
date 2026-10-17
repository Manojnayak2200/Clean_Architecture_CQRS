using LeaveService.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaveService.Application.Interfaces
{
    public interface ILeaveDbContext
    {
        DbSet<LeaveRequest> LeaveRequests { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}

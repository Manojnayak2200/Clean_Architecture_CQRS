using LeaveService.Application.Interfaces;
using LeaveService.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaveService.Infrastructure.Persistence
{
    public class LeaveDbContext : DbContext, ILeaveDbContext
    {
        public LeaveDbContext(DbContextOptions<LeaveDbContext> options) : base(options) { }

        public DbSet<LeaveRequest> LeaveRequests { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LeaveRequest>(entity =>
            {
                entity.HasKey(l => l.Id);
                entity.Property(l => l.Reason).HasMaxLength(500);
                entity.Property(l => l.ApproverComments).HasMaxLength(500);
                entity.HasIndex(l => l.EmployeeId);
                entity.HasIndex(l => l.Status);
            });
        }
    }
}

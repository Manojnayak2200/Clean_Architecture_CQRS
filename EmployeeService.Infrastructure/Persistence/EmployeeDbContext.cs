using EmployeeService.Application.Interfaces;
using EmployeeService.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeService.Infrastructure.Persistence
{
    public class EmployeeDbContext : DbContext, IEmployeeDbContext
    {
        public EmployeeDbContext(DbContextOptions<EmployeeDbContext> options) : base(options) { }

        public DbSet<Employee> Employees { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(150);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.Department).HasMaxLength(50);
                entity.Property(e => e.Salary).HasPrecision(18, 2);
                entity.Property(e => e.DateOfBirth).HasColumnType("datetime(6)");

                // 👇 NEW
                entity.HasIndex(e => e.AuthUserId);
            });
        }
    }
}

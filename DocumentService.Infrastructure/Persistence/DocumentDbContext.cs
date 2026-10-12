using DocumentService.Application.Interfaces;
using DocumentService.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace DocumentService.Infrastructure.Persistence
{
    public class DocumentDbContext : DbContext, IDocumentDbContext
    {
        public DocumentDbContext(DbContextOptions<DocumentDbContext> options) : base(options) { }

        // ✅ Regular public property — no explicit interface impl, no throw
        public DbSet<DocumentFile> DocumentFile { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DocumentFile>(entity =>
            {
                entity.HasKey(d => d.Id);
                entity.Property(d => d.FileName).IsRequired().HasMaxLength(255);
                entity.Property(d => d.ContentType).IsRequired().HasMaxLength(100);
                entity.Property(d => d.StoragePath).IsRequired().HasMaxLength(500);
                entity.Property(d => d.Description).HasMaxLength(500);
                entity.HasIndex(d => d.UploadedByUserId);
                entity.HasIndex(d => d.EmployeeId);
            });
        }
    }
}

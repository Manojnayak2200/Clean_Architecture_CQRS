using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentService.Domain
{
    public class DocumentFile        // ← Must say "class"
    {
        public Guid Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = "application/pdf";
        public long SizeInBytes { get; set; }
        public string StoragePath { get; set; } = string.Empty;
        public Guid UploadedByUserId { get; set; }
        public Guid? EmployeeId { get; set; }
        public string? Description { get; set; }
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    }
}

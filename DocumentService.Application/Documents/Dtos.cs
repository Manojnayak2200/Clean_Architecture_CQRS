using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentService.Application.Documents
{
    public class DocumentResponse
    {
        public Guid Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public long SizeInBytes { get; set; }
        public Guid UploadedByUserId { get; set; }
        public Guid? EmployeeId { get; set; }
        public string? Description { get; set; }
        public DateTime UploadedAt { get; set; }
    }
}

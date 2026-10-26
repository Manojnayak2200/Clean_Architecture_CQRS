using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeService.Application.Interfaces
{
    public class DocumentSummaryDto
    {
        public Guid Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public long SizeInBytes { get; set; }
        public DateTime UploadedAt { get; set; }
    }

    public interface IDocumentServiceClient
    {
        Task<List<DocumentSummaryDto>> GetDocumentsByUserIdAsync();
    }
}

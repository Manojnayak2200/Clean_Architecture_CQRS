using DocumentService.Application.Documents.Commands;
using DocumentService.Application.Documents.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DocumentService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DocumentsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private const long MaxFileSize = 5 * 1024 * 1024;   // 5 MB

        public DocumentsController(IMediator mediator) => _mediator = mediator;

        // Helper: extract user id + admin flag from JWT
        private Guid GetCurrentUserId()
        {
            var sub = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                   ?? User.FindFirst("sub")?.Value;
            return Guid.TryParse(sub, out var id) ? id : Guid.Empty;
        }

        private bool IsAdmin() =>
            User.IsInRole("Admin");

        // ============================================================
        #region 👤 SELF-SERVICE — Upload / List / Download Own Docs
        // ============================================================

        // POST /api/documents/Upload
        [HttpPost("Upload")]
        [RequestSizeLimit(MaxFileSize)]
        public async Task<IActionResult> Upload(
            IFormFile file,
            [FromForm] Guid? employeeId,
            [FromForm] string? description)
        {
            if (file is null || file.Length == 0)
                return BadRequest(new { message = "File is required." });

            if (file.Length > MaxFileSize)
                return BadRequest(new { message = "File exceeds 5 MB limit." });

            if (!string.Equals(Path.GetExtension(file.FileName), ".pdf", StringComparison.OrdinalIgnoreCase))
                return BadRequest(new { message = "Only PDF files are allowed." });

            var userId = GetCurrentUserId();
            if (userId == Guid.Empty) return Unauthorized();

            try
            {
                using var stream = file.OpenReadStream();

                var id = await _mediator.Send(new UploadDocumentCommand
                {
                    FileStream = stream,
                    FileName = file.FileName,
                    ContentType = file.ContentType,
                    SizeInBytes = file.Length,
                    UploadedByUserId = userId,
                    EmployeeId = employeeId,
                    Description = description
                });

                return Ok(new { Id = id });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET /api/documents/MyDocuments
        [HttpGet("MyDocuments")]
        public async Task<IActionResult> GetMine()
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty) return Unauthorized();

            var result = await _mediator.Send(new GetMyDocumentsQuery { UserId = userId });
            return Ok(result);
        }

        // GET /api/documents/Download/{id}
        [HttpGet("Download/{id}")]
        public async Task<IActionResult> Download(Guid id)
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty) return Unauthorized();

            try
            {
                var result = await _mediator.Send(new DownloadDocumentQuery
                {
                    DocumentId = id,
                    RequestedByUserId = userId,
                    IsAdmin = IsAdmin()
                });

                if (result is null) return NotFound();

                return File(result.Stream, result.ContentType, result.FileName);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
        }

        // DELETE /api/documents/Delete/{id}
        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty) return Unauthorized();

            try
            {
                var success = await _mediator.Send(new DeleteDocumentCommand
                {
                    DocumentId = id,
                    RequestedByUserId = userId,
                    IsAdmin = IsAdmin()
                });

                return success
                    ? Ok(new { message = "Document deleted successfully." })
                    : NotFound(new { message = "Document not found." });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
        }

        #endregion

        // ============================================================
        #region 🔐 ADMIN ONLY
        // ============================================================

        // GET /api/documents/AllDocuments
        [HttpGet("AllDocuments")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _mediator.Send(new GetAllDocumentsQuery());
            return Ok(result);
        }

        #endregion
    }
}

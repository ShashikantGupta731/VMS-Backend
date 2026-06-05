using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using backend.Services;
using System.IO;
using System.Threading.Tasks;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UploadController : ControllerBase
    {
        private readonly IFileService _fileService;
        private readonly IWebHostEnvironment _env;

        public UploadController(IFileService fileService, IWebHostEnvironment env)
        {
            _fileService = fileService;
            _env = env;
        }

        /// <summary>
        /// General file upload endpoint for generic attachments.
        /// Saves to wwwroot/uploads/general by default.
        /// </summary>
        [HttpPost]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> UploadAsync([FromForm] IFormFile file, [FromForm] string folder = "general")
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest(new { success = false, message = "No file uploaded." });
                }

                // Sanitize folder name to prevent path traversal
                if (folder.Contains("..") || folder.Contains("/") || folder.Contains("\\"))
                {
                    folder = "general";
                }

                var dbPath = await _fileService.SaveFileAsync(file, folder);

                if (string.IsNullOrEmpty(dbPath))
                {
                    return StatusCode(500, new { success = false, message = "Failed to save the file." });
                }

                var fileName = Path.GetFileName(dbPath);

                return Ok(new { success = true, dbPath, fileName });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Internal server error: {ex.Message}" });
            }
        }

        /// <summary>
        /// Secure file download endpoint.
        /// </summary>
        [HttpGet("DownloadFile")]
        public IActionResult DownloadFile([FromQuery] string filePath)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(filePath))
                {
                    return BadRequest("File path is required.");
                }

                // Remove leading slash if present to cleanly combine with WebRootPath
                var relativePath = filePath.TrimStart('/');
                var fullPath = Path.Combine(_env.WebRootPath, relativePath);

                if (!System.IO.File.Exists(fullPath))
                {
                    return NotFound("File not found.");
                }

                var fileName = Path.GetFileName(fullPath);
                
                // Content disposition as attachment ensures it downloads instead of rendering in browser
                return PhysicalFile(fullPath, "application/octet-stream", fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}

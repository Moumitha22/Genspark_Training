using ClinicManagementSystem.Contexts;
using ClinicManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;

namespace ClinicManagementSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FileHandleController : ControllerBase
    {
        private readonly ClinicContext _context;

        public FileHandleController(ClinicContext context)
        {
            _context = context;
        }

        // POST: api/FileHandle/upload
        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            try
            {
                using var memoryStream = new MemoryStream();
                await file.CopyToAsync(memoryStream);

                var fileFormat = new FileFormat
                {
                    Name = file.FileName,
                    ContentType = file.ContentType,
                    Data = memoryStream.ToArray()
                };

                _context.FileFormats.Add(fileFormat);
                await _context.SaveChangesAsync();

                return Ok(new { message = "File uploaded and stored successfully.", fileId = fileFormat.Id });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/FileHandle/download/5
        [HttpGet("download/{id}")]
        public async Task<IActionResult> Download(int id)
        {
            try
            {
                var fileFormat = await _context.FileFormats.FindAsync(id);
                if (fileFormat == null)
                    return NotFound("File not found.");

                return File(fileFormat.Data, fileFormat.ContentType, fileFormat.Name);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}

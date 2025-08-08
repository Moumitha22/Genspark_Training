using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;

namespace ClinicManagementSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FileController : ControllerBase
    {
        private static string _savedFilePath;

        // POST: api/file/upload
        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            var filePath = Path.Combine(Path.GetTempPath(), file.FileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            _savedFilePath = filePath; // Store file path temporarily

            return Ok(new { message = "File uploaded successfully.", path = filePath });
        }

        // POST: api/file/path
        [HttpPost("path")]
        public IActionResult UploadPath([FromBody] string filePath)
        {
            if (!System.IO.File.Exists(filePath))
                return NotFound("File not found.");

            _savedFilePath = filePath;

            return Ok(new { message = "Path received.", path = filePath });
        }

        // GET: api/file
        [HttpGet]
        public IActionResult GetFileAsBytes()
        {
            if (string.IsNullOrEmpty(_savedFilePath) || !System.IO.File.Exists(_savedFilePath))
                return NotFound("No file available.");

            var fileBytes = System.IO.File.ReadAllBytes(_savedFilePath);
            return Ok(fileBytes);
        }
    }
}

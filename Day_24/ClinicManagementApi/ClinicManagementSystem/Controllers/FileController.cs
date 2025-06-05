using ClinicManagementSystem.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;

namespace ClinicManagementSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FileController : ControllerBase
    {
        private readonly static string _uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "UploadedFiles");

        public FileController()
        {
            if (!Directory.Exists(_uploadPath))
            {
                Directory.CreateDirectory(_uploadPath);
            }
        }


        // POST: api/file/upload
        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadFile([FromForm] FileUploadDto model)
        {
            if (model.File == null || model.File.Length == 0)
                return BadRequest("No file uploaded.");

            var filePath = Path.Combine(_uploadPath, model.File.FileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await model.File.CopyToAsync(stream);
            }

            return Ok(new { message = $"File {model.File.FileName} uploaded successfully.", path = filePath });
        }
        
        // GET: api/file
        [HttpGet("get-bytes/{filename}")]
        public IActionResult GetFileAsBytes(string filename)
        {
            var filePath = Path.Combine(_uploadPath, filename);

            if (string.IsNullOrEmpty(filePath) || !System.IO.File.Exists(filePath))
                return NotFound("No file available.");

            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            return Ok(fileBytes);
        }

        [HttpGet("get-content/{filename}")]
        public async Task<IActionResult> GetFile(string filename)
        {
            var filePath = Path.Combine(_uploadPath, filename);

            if (string.IsNullOrEmpty(filePath) || !System.IO.File.Exists(filePath))
                return NotFound("File not found.");

            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
            var contentType = "application/octet-stream";
            return new FileContentResult(fileBytes, contentType);

        }

        [HttpGet("download/{file-name}")]
        public IActionResult DownloadFile(string filename)
        {
            var filePath = Path.Combine(_uploadPath, filename);

            if (string.IsNullOrEmpty(filePath) || !System.IO.File.Exists(filePath))
                return NotFound("No file available.");

            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            var fileName = Path.GetFileName(filePath);
            var contentType = "application/octet-stream"; 

            return File(fileBytes, contentType, fileName);
        }

    }
}

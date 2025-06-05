using DocShareApi.Interfaces;
using DocShareApi.Models;
using DocShareApi.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using DocShareApi.Misc;
using System.Security.Claims;

namespace DocShareApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentsController : ControllerBase
    {
        private readonly IRepository<Guid, Document> _documentRepository;
        private readonly IRepository<string, User> _userRepository;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly IWebHostEnvironment _env;

        public DocumentsController(
            IRepository<Guid, Document> documentRepository,
            IRepository<string, User> userRepository,
            IHubContext<NotificationHub> hubContext,
            IWebHostEnvironment env)
        {
            _documentRepository = documentRepository;
            _userRepository = userRepository;
            _hubContext = hubContext;
            _env = env;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllDocuments()
        {
            var docs = await _documentRepository.GetAll();
            return Ok(docs);
        }

        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        [Authorize(Roles = "HRAdmin")]
        public async Task<IActionResult> UploadDocument([FromForm] DocumentUploadDto dto)
        {
            var uploaderEmail = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (uploaderEmail == null)
                return Unauthorized("Invalid token");

            if (dto.File == null || dto.File.Length == 0)
                return BadRequest("File is required.");

            var uploadsPath = Path.Combine(_env.ContentRootPath, "UploadedDocs");
            if (!Directory.Exists(uploadsPath))
                Directory.CreateDirectory(uploadsPath);

            var fileName = $"{Guid.NewGuid()}_{dto.File.FileName}";
            var filePath = Path.Combine(uploadsPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await dto.File.CopyToAsync(stream);
            }

            var document = new Document
            {
                DocumentId = Guid.NewGuid(),
                FileName = dto.File.FileName,
                FilePath = filePath,
                UploadedBy = uploaderEmail,
                UploadedAt = DateTime.UtcNow,
                Description = dto.Description,
                Status = "Active"
            };

            var addedDoc = await _documentRepository.Add(document);

            await _hubContext.Clients.All.SendAsync("NewFileUploaded", addedDoc.FileName, addedDoc.UploadedBy);

            return Ok(addedDoc);
        }
        
        [HttpGet("download/{id}")]
        [Authorize]
        public async Task<IActionResult> DownloadDocument(Guid id)
        {
            var document = await _documentRepository.Get(id);
            if (document == null || !System.IO.File.Exists(document.FilePath))
                return NotFound("Document not found");

            var fileBytes = await System.IO.File.ReadAllBytesAsync(document.FilePath);
            var contentType = "application/octet-stream"; 
            return File(fileBytes, contentType, document.FileName);
        }

    }
}

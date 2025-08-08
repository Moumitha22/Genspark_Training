using ChienVHShopOnline.Models.DTOs;
using ChienVHShopOnline.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChienVHShopOnline.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewsController : ControllerBase
    {
        private readonly INewsService _service;

        public NewsController(INewsService service)
        {
            _service = service;
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<NewsResponseDto>>> GetAll()
        {
            var newsList = await _service.GetAllNews();
            return Ok(newsList);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<NewsResponseDto>> Get(int id)
        {
            var news = await _service.GetNewsById(id);
            if (news == null) return NotFound();
            return Ok(news);
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<NewsResponseDto>> Create(NewsRequestDto dto)
        {
            var created = await _service.CreateNews(dto);
            return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<NewsResponseDto>> Update(int id, NewsRequestDto dto)
        {
            try
            {
                var updated = await _service.UpdateNews(id, dto);
                return Ok(updated);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<NewsResponseDto>> Delete(int id)
        {
            try
            {
                var deleted = await _service.DeleteNews(id);
                return Ok(deleted);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("export/csv")]
        public async Task<IActionResult> ExportToCsv()
        {
            var csvBytes = await _service.ExportToCsv();
            var fileName = $"NewsListing_{DateTime.UtcNow:yyyyMMddHHmmss}.csv";
            return File(csvBytes, "text/csv", fileName);
        }

        [HttpGet("export/excel")]
        public async Task<IActionResult> ExportToExcel()
        {
            var fileBytes = await _service.ExportToExcel();
            return File(fileBytes, 
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", 
                "news.xlsx");
        }


    }
}

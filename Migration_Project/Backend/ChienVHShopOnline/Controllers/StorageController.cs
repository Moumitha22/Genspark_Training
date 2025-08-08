using ChienVHShopOnline.Interfaces;
using ChienVHShopOnline.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace ChienVHShopOnline.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StorageController : ControllerBase
    {
        private readonly IStorageService _service;

        public StorageController(IStorageService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<StorageResponseDto>>> GetAll()
        {
            var result = await _service.GetAll();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<StorageResponseDto>> Get(int id)
        {
            try
            {
                var result = await _service.Get(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<StorageResponseDto>> Create(StorageRequestDto Dto)
        {
            var result = await _service.Add(Dto);
            return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<StorageResponseDto>> Update(int id, StorageRequestDto Dto)
        {
            try
            {
                var result = await _service.Update(id, Dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<StorageResponseDto>> Delete(int id)
        {
            try
            {
                var result = await _service.Delete(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}

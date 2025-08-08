using AutoMapper;
using ChienVHShopOnline.Interfaces;
using ChienVHShopOnline.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace ChienVHShopOnline.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ColorController : ControllerBase
    {
        private readonly IColorService _colorService;

        public ColorController(IColorService colorService)
        {
            _colorService = colorService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ColorResponseDto>>> GetAll()
        {
            var colors = await _colorService.GetAllAsync();
            return Ok(colors);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ColorResponseDto>> GetById(int id)
        {
            var color = await _colorService.GetByIdAsync(id);
            if (color == null) return NotFound();
            return Ok(color);
        }

        [HttpPost]
        public async Task<ActionResult<ColorResponseDto>> Create(ColorRequestDto dto)
        {
            var created = await _colorService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, ColorRequestDto dto)
        {
            var success = await _colorService.UpdateAsync(id, dto);
            return success ? NoContent() : NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _colorService.DeleteAsync(id);
            return success ? NoContent() : NotFound();
        }
    }
}

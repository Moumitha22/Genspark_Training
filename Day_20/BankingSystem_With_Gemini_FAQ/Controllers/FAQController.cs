using BankManagementSystem.Services;
using BankManagementSystem.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace BankManagementSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FaqController : ControllerBase
    {
        private readonly FAQService _geminiService;

        public FaqController(FAQService geminiService)
        {
            _geminiService = geminiService;
        }

        [HttpPost]
        public async Task<ActionResult<FaqResponse>> Post([FromBody] FaqRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Question))
                return BadRequest("Question cannot be empty.");

            try
            {
                var answer = await _geminiService.GetBankingAnswerAsync(request.Question);
                return Ok(new FaqResponse { Answer = answer });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error generating response: {ex.Message}");
            }
        }

        [HttpGet("models")]
        public async Task<IActionResult> GetModels()
        {
            try
            {
                var result = await _geminiService.ListModelsAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}

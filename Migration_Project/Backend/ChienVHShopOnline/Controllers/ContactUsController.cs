using ChienVHShopOnline.Interfaces;
using ChienVHShopOnline.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace ChienVHShopOnline.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContactUsController : ControllerBase
    {
        private readonly ICaptchaService _captchaService;
        private readonly IContactUsService _contactService;

        public ContactUsController(ICaptchaService captchaService, IContactUsService contactService)
        {
            _captchaService = captchaService;
            _contactService = contactService;
        }

        [HttpPost]
        public async Task<IActionResult> Submit([FromBody] ContactUsRequestDto dto)
        {
            if (!await _captchaService.VerifyTokenAsync(dto.CaptchaToken))
            {
                return BadRequest(new { message = "Captcha verification failed." });
            }

            await _contactService.SubmitAsync(dto);
            return Ok(new { message = "Your query has been submitted successfully!" });
        }
    }
}


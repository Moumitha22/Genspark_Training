using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using PropFinderApi.Exceptions;
using PropFinderApi.Interfaces;
using PropFinderApi.Models.DTOs;
using System.Security.Claims;

namespace PropFinderApi.Controllers
{
    [EnableRateLimiting("PerUserLimiter")]
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class ListerProfileController : ControllerBase
    {
        private readonly IListerProfileService _listerProfileService;
        private readonly IApiResponseMapper _responseMapper;

        public ListerProfileController(IListerProfileService agentService, IApiResponseMapper responseMapper)
        {
            _listerProfileService = agentService;
            _responseMapper = responseMapper;
        }

        [HttpPost]
        [Authorize(Roles = "Lister")]
        public async Task<IActionResult> Create([FromBody] ListerProfileAddRequestDto dto)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var created = await _listerProfileService.CreateListerProfileAsync(dto, userId);
            var response = _responseMapper.MapToOkResponse("Lister profile created successfully", created);
            return Ok(response);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var listerProfiles = await _listerProfileService.GetAllAsync();

            var response = _responseMapper.MapToOkResponse("All Lister profiles fetched successfully", listerProfiles);
            return Ok(response);
        }

        [HttpGet("{listerProfileId:guid}")]
        [Authorize(Roles = "Admin,Agent")]
        public async Task<IActionResult> GetById(Guid listerProfileId)
        {
            if (listerProfileId == Guid.Empty)
                throw new BadRequestException("Invalid Lister profile ID.");

            var listerProfile = await _listerProfileService.GetByIdAsync(listerProfileId);
            var response = _responseMapper.MapToOkResponse("Lister Profile fetched by ID", listerProfile);
            return Ok(response);
        }

        [HttpGet("by-lister/{listerId:guid}")]
        [Authorize(Roles = "Lister,Admin")]
        public async Task<IActionResult> GetByListerId(Guid listerId)
        {
            var listerProfile = await _listerProfileService.GetListerProfileByListerIdAsync(listerId);
            if (listerProfile == null)
                throw new NotFoundException("Lister profile not found");

            return Ok(_responseMapper.MapToOkResponse("Lister profile fetched successfully", listerProfile));
        }

        [HttpPut("{listerProfileId:guid}")]
        [Authorize(Roles = "Lister,Admin")]
        public async Task<IActionResult> Update(Guid listerProfileId, [FromBody] ListerProfileAddRequestDto dto)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var userRole = User.FindFirstValue(ClaimTypes.Role)!;

            var updated = await _listerProfileService.UpdateListerProfileAsync(listerProfileId, dto, userId, userRole);
            var response = _responseMapper.MapToOkResponse("Lister profile updated successfully", updated);
            return Ok(response);
        }
        
        [HttpGet("is-complete")]
        [Authorize(Roles = "Lister")]
        public async Task<IActionResult> IsProfileComplete()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var isComplete = await _listerProfileService.IsProfileCompleteAsync(userId);

            return Ok(new { isComplete });
        }
    }
}

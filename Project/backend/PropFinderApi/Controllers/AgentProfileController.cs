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
    public class AgentProfileController : ControllerBase
    {
        private readonly IAgentProfileService _agentService;
        private readonly IPaginationService _paginationService;
        private readonly IApiResponseMapper _responseMapper;

        public AgentProfileController(IAgentProfileService agentService, IPaginationService paginationService, IApiResponseMapper responseMapper)
        {
            _agentService = agentService;
            _paginationService = paginationService;
            _responseMapper = responseMapper;
        }

        [HttpPost]
        [Authorize(Roles = "Agent")]
        public async Task<IActionResult> Create([FromBody] AgentProfileAddRequestDto dto)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var created = await _agentService.CreateAgentProfileAsync(dto, userId);
            var response = _responseMapper.MapToOkResponse("Agent profile created successfully", created);
            return Ok(response);
        }

        [HttpGet]
         [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (page <= 0 || pageSize <= 0)
                throw new BadRequestException("Page and pageSize must be greater than 0");

            var agentProfiles = await _agentService.GetAllAsync();

            var (paginatedAgentProfiles, pagination) = _paginationService.ApplyPagination(agentProfiles, page, pageSize);

            var response = _responseMapper.MapToOkResponse("All agent profiles fetched successfully", paginatedAgentProfiles, pagination);
            return Ok(response);
        }

        [HttpGet("{agentProfileId:guid}")]
         [Authorize(Roles = "Admin,Agent")]
        public async Task<IActionResult> GetById(Guid agentProfileId)
        {
            if (agentProfileId== Guid.Empty)
                throw new BadRequestException("Invalid agent profile ID.");

            var agentProfile = await _agentService.GetByIdAsync(agentProfileId);
            var response = _responseMapper.MapToOkResponse("Agent Profile fetched by ID", agentProfile);
            return Ok(response);
        }

        [HttpGet("by-agent/{agentId:guid}")]
        [Authorize(Roles = "Agent,Admin")]
        public async Task<IActionResult> GetByAgentId(Guid agentId)
        {
            var agentProfile = await _agentService.GetAgentProfileByAgentIdAsync(agentId);
            if (agentProfile == null)
                throw new NotFoundException("Agent profile not found");

            return Ok(_responseMapper.MapToOkResponse("Agent profile fetched successfully", agentProfile));
        }

        [HttpPut("{agentProfileId:guid}")]
        [Authorize(Roles = "Agent,Admin")]
        public async Task<IActionResult> Update(Guid agentProfileId, [FromBody] AgentProfileAddRequestDto dto)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var userRole = User.FindFirstValue(ClaimTypes.Role)!;

            var updated = await _agentService.UpdateAgentProfileAsync(agentProfileId, dto, userId, userRole);
            var response = _responseMapper.MapToOkResponse("Agent profile updated successfully", updated);
            return Ok(response);
        }
    }
}

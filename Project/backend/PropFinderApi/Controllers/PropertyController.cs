using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using PropFinderApi.Exceptions;
using PropFinderApi.Interfaces;
using PropFinderApi.Models;
using PropFinderApi.Models.DTOs;
using System.Security.Claims;

namespace PropFinderApi.Controllers
{
    [EnableRateLimiting("PerUserLimiter")]
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class PropertyController : ControllerBase
    {
        private readonly IPropertyService _propertyService;
        private readonly IPaginationService _paginationService;
        private readonly IApiResponseMapper _mapper;

        public PropertyController(IPropertyService propertyService, IPaginationService paginationService, IApiResponseMapper mapper)
        {
            _propertyService = propertyService;
            _paginationService = paginationService;
            _mapper = mapper;
        }

        [HttpPost]
        [Authorize(Roles = "Agent")]
        public async Task<IActionResult> Create([FromBody] PropertyAddRequestDto dto)
        {
            var agentId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var property = await _propertyService.CreatePropertyAsync(dto, agentId);
            var response = _mapper.MapToOkResponse("Property created successfully", property);
            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (page <= 0 || pageSize <= 0)
                throw new BadRequestException("Page and pageSize must be greater than 0");

            var properties = await _propertyService.GetAllPropertiesAsync();

            var (paginatedProperties, pagination) = _paginationService.ApplyPagination(properties, page, pageSize);

            var response = _mapper.MapToOkResponse("All properties fetched successfully", paginatedProperties, pagination);
            return Ok(response);
        }

        [HttpGet("{propertyId:guid}")]
        public async Task<IActionResult> GetById(Guid propertyId)
        {
            if (propertyId == Guid.Empty)
                throw new BadRequestException("Invalid property ID.");

            var property = await _propertyService.GetPropertyByIdAsync(propertyId);
            var response = _mapper.MapToOkResponse("Property fetched by ID", property);
            return Ok(response);
        }

        [HttpGet("by-agent/{agentId:guid}")]
        [Authorize(Roles = "Admin,Agent")]
        public async Task<IActionResult> GetPropertiesByAgentId(Guid agentId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (agentId == Guid.Empty)
                throw new BadRequestException("Invalid agent ID.");

            if (page <= 0 || pageSize <= 0)
                throw new BadRequestException("Page and pageSize must be greater than 0");


            var properties = await _propertyService.GetPropertiesByAgentIdAsync(agentId);

            var (paginatedProperties, pagination) = _paginationService.ApplyPagination(properties, page, pageSize);

            var response = _mapper.MapToOkResponse("Properties fetched by Agent ID", paginatedProperties, pagination);
            return Ok(response);
        }

        [HttpGet("sold")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetSoldProperties([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (page <= 0 || pageSize <= 0)
                throw new BadRequestException("Page and pageSize must be greater than 0");

            var properties = await _propertyService.GetSoldProperties();

            var (paginatedProperties, pagination) = _paginationService.ApplyPagination(properties, page, pageSize);

            var response = _mapper.MapToOkResponse("All properties fetched successfully", paginatedProperties, pagination);
            return Ok(response);
        }


        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] PropertySearchModel searchModel, [FromQuery] SortModel sortModel, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (page <= 0 || pageSize <= 0)
                throw new BadRequestException("Page and pageSize must be greater than 0");

            var properties = await _propertyService.SearchPropertiesAsync(searchModel, sortModel);

            var (paginatedProperties, pagination) = _paginationService.ApplyPagination(properties, page, pageSize);

            var response = _mapper.MapToOkResponse("All properties fetched successfully", paginatedProperties, pagination);
            return Ok(response);
        }
        

        [HttpPut("{propertyId:guid}")]
        [Authorize(Roles = "Agent,Admin")]
        public async Task<IActionResult> UpdateProperty(Guid propertyId, [FromBody] PropertyUpdateRequestDto dto)
        {
            if (propertyId == Guid.Empty)
                throw new BadRequestException("Invalid property ID.");

            var requesterId = GetUserId();
            var userRole = GetUserRole();

            var updatedProperty = await _propertyService.UpdatePropertyAsync(propertyId, dto, requesterId, userRole);

            var response = _mapper.MapToOkResponse("Property updated successfully", updatedProperty);
            return Ok(response);
        }

        [HttpPatch("{propertyId}/status/")]
        [Authorize(Roles = "Agent,Admin")]
        public async Task<IActionResult> UpdateStatus(Guid propertyId, [FromQuery] string newStatus)
        {
            var requesterId = GetUserId();
            var userRole = GetUserRole();

            await _propertyService.UpdatePropertyStatusAsync(propertyId, newStatus, requesterId, userRole);

            var response = _mapper.MapToOkResponse("Property status updated successfully");
            return Ok(response);
        }


        [HttpDelete("{propertyId:guid}")]
        [Authorize(Roles = "Agent,Admin")]
        public async Task<IActionResult> Delete(Guid propertyId)
        {
            if (propertyId == Guid.Empty)
                throw new BadRequestException("Invalid property ID.");

            var requesterId = GetUserId();
            var userRole = GetUserRole();

            await _propertyService.DeletePropertyAsync(propertyId, requesterId, userRole);
            var response = _mapper.MapToOkResponse("Property deleted successfully");
            return Ok(response);
        }

        private Guid GetUserId()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            return userId;
        }

        private string GetUserRole()
        {
            var role = User.FindFirstValue(ClaimTypes.Role);
            return role;
        }

    }
}

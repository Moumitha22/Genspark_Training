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
    public class PropertyController : ControllerBase
    {
        private readonly IPropertyService _propertyService;
        private readonly IApiResponseMapper _mapper;

        public PropertyController(IPropertyService propertyService, IApiResponseMapper mapper)
        {
            _propertyService = propertyService;
            _mapper = mapper;
        }

        [HttpPost]
        [Authorize(Roles = "Lister")]
        public async Task<IActionResult> Create([FromBody] PropertyAddRequestDto dto)
        {
            var listerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var property = await _propertyService.CreatePropertyAsync(dto, listerId);
            var response = _mapper.MapToOkResponse("Property created successfully", property);
            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var properties = await _propertyService.GetAllPropertiesAsync();

            var response = _mapper.MapToOkResponse("All properties fetched successfully", properties);
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

        [HttpGet("by-lister/{listerId:guid}")]
        [Authorize(Roles = "Lister,Admin")]
        public async Task<IActionResult> GetPropertiesByLister(Guid listerId,[FromQuery] PaginationModel pagination)
        {
            if (listerId == Guid.Empty)
                throw new BadRequestException("Invalid agent ID.");

            var properties = await _propertyService.GetPropertiesByListerIdAsync(listerId, pagination);

            var response = _mapper.MapToOkResponse("Properties fetched by Lister ID", properties);
            return Ok(response);
        }

        [HttpGet("sold")]
        [Authorize(Roles = "Admin,Lister")]
        public async Task<IActionResult> GetSoldProperties()
        {
            var properties = await _propertyService.GetSoldProperties();

            var response = _mapper.MapToOkResponse("All properties fetched successfully", properties);
            return Ok(response);
        }


        [HttpGet("search")]
        public async Task<IActionResult> BasicSearch(
            [FromQuery] BasicPropertySearchModel model,
            [FromQuery] SortModel sort,
            [FromQuery] PaginationModel pagination)
        {
            var result = await _propertyService.BasicSearchPropertiesAsync(model, sort, pagination);
            return Ok(new ApiResponse<PaginatedResult<PropertyResponseDto>>
            {
                Success = true,
                Message = "Properties fetched successfully",
                Data = result
            });
        }


        [HttpPost("search")]
        public async Task<IActionResult> AdvancedSearch(
            [FromBody] AdvancedPropertySearchModel searchModel,
            [FromQuery] SortModel sortModel,
            [FromQuery] PaginationModel paginationModel)
        {
            var result = await _propertyService.AdvancedSearchPropertiesAsync(searchModel, sortModel, paginationModel);

            var response = new ApiResponse<PaginatedResult<PropertyResponseDto>>
            {
                Success = true,
                Message = "Properties fetched successfully",
                Data = result,
            };

            return Ok(response);
        }


        [HttpPut("{propertyId:guid}")]
        [Authorize(Roles = "Lister,Admin")]
        public async Task<IActionResult> UpdateProperty(Guid propertyId, [FromBody] PropertyAddRequestDto dto)
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
        [Authorize(Roles = "Lister,Admin")]
        public async Task<IActionResult> UpdateStatus(Guid propertyId, [FromQuery] string newStatus)
        {
            var requesterId = GetUserId();
            var userRole = GetUserRole();

            await _propertyService.UpdatePropertyStatusAsync(propertyId, newStatus, requesterId, userRole);

            var response = _mapper.MapToOkResponse("Property status updated successfully");
            return Ok(response);
        }


        [HttpDelete("{propertyId:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid propertyId)
        {
            if (propertyId == Guid.Empty)
                throw new BadRequestException("Invalid property ID.");

            await _propertyService.DeletePropertyAsync(propertyId);
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

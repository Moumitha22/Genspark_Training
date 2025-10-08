using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using PropFinderApi.Interfaces;
using PropFinderApi.Exceptions;

namespace PropFinderApi.Controllers.v2
{
    [ApiController]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
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
    }
}

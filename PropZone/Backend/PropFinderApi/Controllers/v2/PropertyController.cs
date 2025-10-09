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
        private readonly IApiResponseMapper _mapper;

        public PropertyController(IPropertyService propertyService, IApiResponseMapper mapper)
        {
            _propertyService = propertyService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var properties = await _propertyService.GetAllPropertiesAsync();

            var response = _mapper.MapToOkResponse("All properties fetched successfully", properties);
            return Ok(response);
        }
    }
}

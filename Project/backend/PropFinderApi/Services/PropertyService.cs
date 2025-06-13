using PropFinderApi.Exceptions;
using PropFinderApi.Interfaces;
using PropFinderApi.Mappers;
using PropFinderApi.Misc;
using PropFinderApi.Models;
using PropFinderApi.Models.DTOs;
using Microsoft.AspNetCore.SignalR;

namespace PropFinderApi.Services
{
    public class PropertyService : IPropertyService
    {
        private readonly IPropertyRepository _propertyRepository;
        private readonly IAgentProfileRepository _agentProfileRepository;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly PropertyMapper _propertyMapper;

        public PropertyService(IPropertyRepository propertyRepository, IAgentProfileRepository agentProfileRepository, IHubContext<NotificationHub> hubContext)
        {
            _propertyRepository = propertyRepository;
            _agentProfileRepository = agentProfileRepository;
            _propertyMapper = new PropertyMapper();
            _hubContext = hubContext;
        }

        public async Task<Property> CreatePropertyAsync(PropertyAddRequestDto propertyAddRequestDto, Guid agentId)
        {
            var profile = await _agentProfileRepository.GetByUserIdAsync(agentId);
            if (profile == null)
                throw new NotFoundException("You must complete your agent profile before adding a property.");

            if (string.IsNullOrWhiteSpace(profile.BusinessPhoneNumber))
                throw new BadRequestException("Incomplete agent profile. Please fill all required fields.");

            var property = _propertyMapper.MapPropertyAddRequestDtoToProperty(propertyAddRequestDto, agentId);
            property = await _propertyRepository.Add(property);
            await _hubContext.Clients.All.SendAsync("NewPropertyUploaded", property.Title, property.Location);

            return property;
        }

        public async Task<IEnumerable<PropertyResponseDto>> GetAllPropertiesAsync()
        {
            var properties = await _propertyRepository.GetAll();
            return properties.Select(property => _propertyMapper.MapPropertyToPropertyResponseDto(property));
        }

        public async Task<IEnumerable<PropertyResponseDto>> GetSoldProperties()
        {
            var properties = await _propertyRepository.GetSoldProperties();
            return properties.Select(property => _propertyMapper.MapPropertyToPropertyResponseDto(property));
        }

        public async Task<PropertyResponseDto> GetPropertyByIdAsync(Guid id)
        {
            var property = await _propertyRepository.Get(id);

            return _propertyMapper.MapPropertyToPropertyResponseDto(property);
        }

        public async Task<IEnumerable<PropertyResponseDto>> GetPropertiesByAgentIdAsync(Guid agentId)
        {
            var properties = await _propertyRepository.GetByAgentIdAsync(agentId, false);
            return properties.Select(property => _propertyMapper.MapPropertyToPropertyResponseDto(property));
        }

        public async Task<PropertyResponseDto> UpdatePropertyAsync(Guid propertyId, PropertyUpdateRequestDto dto, Guid requesterId,  string userRole)
        {
            var property = await _propertyRepository.Get(propertyId);

            if (userRole == "Agent" && property.AgentId != requesterId)
                throw new UnauthorizedException("You can only update your own property status");

            property = _propertyMapper.UpdatePropertyFromDto(property, dto);

            var updated = await _propertyRepository.Update(propertyId, property);
            return _propertyMapper.MapPropertyToPropertyResponseDto(updated);
        }

        public async Task UpdatePropertyStatusAsync(Guid propertyId, string newStatus, Guid requesterId, string userRole)
        {
            var property = await _propertyRepository.Get(propertyId);

            if (userRole == "Agent" && property.AgentId != requesterId)
                throw new UnauthorizedException("You can only update your own property status");

            await _propertyRepository.UpdateStatusAsync(propertyId, newStatus);
        }

        public async Task DeletePropertyAsync(Guid propertyId, Guid requesterId, string userRole)
        {
            var property = await _propertyRepository.Get(propertyId);

            if (userRole == "Agent" && property.AgentId != requesterId)
                throw new UnauthorizedException("You are not authorized to delete this property.");

            property.IsDeleted = true;
            property.UpdatedAt = DateTime.UtcNow;
            await _propertyRepository.Update(property.Id, property);
        }

        public async Task<IEnumerable<PropertyResponseDto>> SearchPropertiesAsync(PropertySearchModel searchModel, SortModel sortModel)
        {
            var properties = await _propertyRepository.SearchPropertiesAsync(searchModel, sortModel);
            return properties.Select(property => _propertyMapper.MapPropertyToPropertyResponseDto(property));
        }        
    }
}

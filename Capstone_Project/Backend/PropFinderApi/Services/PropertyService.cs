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
        private readonly IPropertyLocationRepository _propertyLocationRepository;
        private readonly IPropertyFeatureService _propertyFeatureService;
        private readonly IListerProfileRepository _listerProfileRepository;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly PropertyMapper _propertyMapper;

        public PropertyService(IPropertyRepository propertyRepository, IListerProfileRepository listerProfileRepository, IPropertyLocationRepository propertyLocationRepository, IPropertyFeatureService propertyFeatureService, IHubContext<NotificationHub> hubContext)
        {
            _propertyRepository = propertyRepository;
            _propertyLocationRepository = propertyLocationRepository;
            _propertyFeatureService = propertyFeatureService;
            _listerProfileRepository = listerProfileRepository;
            _propertyMapper = new PropertyMapper();
            _hubContext = hubContext;
        }

        public async Task<Property> CreatePropertyAsync(PropertyAddRequestDto propertyAddRequestDto, Guid listerId)
        {
            var profile = await _listerProfileRepository.GetByUserIdAsync(listerId);
            if (profile == null)
                throw new NotFoundException("You must complete your lister profile before adding a property.");

            if (string.IsNullOrWhiteSpace(profile.BusinessPhoneNumber))
                throw new BadRequestException("Incomplete lister profile. Please fill all required fields.");

            var property = _propertyMapper.MapPropertyAddRequestDtoToProperty(propertyAddRequestDto, listerId);
            property = await _propertyRepository.Add(property);
            var location = $"{property.Location.Locality}, {property.Location.City}, {property.Location.State}";

            await _hubContext.Clients.Group("Buyers").SendAsync("NewPropertyUploaded", property.Title, location, DateTime.UtcNow.ToString("O"));


            return property;
        }


        public async Task<PropertyResponseDto> UpdatePropertyAsync(Guid propertyId,PropertyAddRequestDto dto,Guid requesterId,string userRole)
        {
            var property = await _propertyRepository.Get(propertyId);

            if (userRole == "Lister" && property.ListerId != requesterId)
                throw new UnauthorizedException("You can only update your own property.");

            _propertyMapper.MapPropertyUpdateRequestDtoToProperty(property, dto);

            await _propertyRepository.Update(property.Id, property);

            await _propertyLocationRepository.UpsertAsync(property.Id, dto.Location);

            await _propertyFeatureService.UpdateFeatureSetAsync(property.Id, dto.Features);
        
            var updated = await _propertyRepository.Get(property.Id);

            return _propertyMapper.MapPropertyToPropertyResponseDto(updated);
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

        
        public async Task<PaginatedResult<PropertyResponseDto>> GetPropertiesByListerIdAsync(Guid listerId, PaginationModel paginationModel)
        {
            var properties = await _propertyRepository.GetByListerIdAsync(listerId, paginationModel);
            return properties;
        }

        public async Task UpdatePropertyStatusAsync(Guid propertyId, string newStatus, Guid requesterId, string userRole)
        {
            var property = await _propertyRepository.Get(propertyId);

            if (userRole == "Lister" && property.ListerId != requesterId)
                throw new UnauthorizedException("You can only update your own property status");

            await _propertyRepository.UpdateStatusAsync(propertyId, newStatus);
        }

        public async Task DeletePropertyAsync(Guid propertyId)
        {
            var property = await _propertyRepository.Get(propertyId);

            property.IsDeleted = true;
            property.UpdatedAt = DateTime.UtcNow;
            await _propertyRepository.Update(property.Id, property);
        }

        public async Task<PaginatedResult<PropertyResponseDto>> BasicSearchPropertiesAsync(BasicPropertySearchModel searchModel, SortModel sortModel, PaginationModel paginationModel)
        {
            return await _propertyRepository.BasicSearchAsync(searchModel, sortModel, paginationModel);
        }

        public async Task<PaginatedResult<PropertyResponseDto>> AdvancedSearchPropertiesAsync(AdvancedPropertySearchModel searchModel, SortModel sortModel, PaginationModel paginationModel)
        {
            return await _propertyRepository.AdvancedSearchAsync(searchModel, sortModel, paginationModel);
        }
    }
    

}

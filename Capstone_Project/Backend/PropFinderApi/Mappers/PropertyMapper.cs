using PropFinderApi.Models;
using PropFinderApi.Models.DTOs;
using PropFinderApi.Models.Enums;

namespace PropFinderApi.Mappers
{
    public class PropertyMapper
    {
        public Property MapPropertyAddRequestDtoToProperty(PropertyAddRequestDto dto, Guid listerId)
        {
            var property = new Property
            {
                Id = Guid.NewGuid(),
                ListerId = listerId,
                Title = SanitizeText(dto.Title),
                Description = SanitizeText(dto.Description ?? ""),
                Price = dto.Price,
                ListerType = dto.ListerType, 
                PropertyType = dto.PropertyType,
                ListingPurpose = dto.ListingPurpose,
                AreaSqFt = dto.AreaSqFt,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Status = ListingStatus.Available,
                IsDeleted = false
            };

            // Map location
            if (dto.Location != null)
            {
                property.Location = new PropertyLocation
                {
                    Id = Guid.NewGuid(),
                    Locality = dto.Location.Locality,
                    City = dto.Location.City,
                    State = dto.Location.State,
                    Latitude = dto.Location.Latitude,
                    Longitude = dto.Location.Longitude,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
            }
            if (dto.Features != null && dto.Features.Any())
            {
                property.Features = dto.Features.Select(f =>
                {
                    // Normalize the dataType to lower for safe comparison
                    var dataType = f.DataType?.ToLower();

                    var isOptionFeature = dataType == "dropdown" || dataType == "multiselect";

                    return new PropertyFeature
                    {
                        Id = Guid.NewGuid(),
                        FeatureId = f.FeatureId,
                        Value = !isOptionFeature ? f.Value : null,
                        OptionId = isOptionFeature ? f.OptionId : null,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        IsDeleted = false
                    };
                }).ToList();
            }

            return property;
        }

        public void MapPropertyUpdateRequestDtoToProperty(Property property, PropertyAddRequestDto dto)
        {
            property.Title = SanitizeText(dto.Title);
            property.Description = SanitizeText(dto.Description ?? "");
            property.Price = dto.Price;
            property.ListerType = dto.ListerType;
            property.PropertyType = dto.PropertyType;
            property.ListingPurpose = dto.ListingPurpose;
            property.AreaSqFt = dto.AreaSqFt;
            property.Status = dto.Status;
            property.UpdatedAt = DateTime.UtcNow;
        }

        public PropertyResponseDto MapPropertyToPropertyResponseDto(Property p)
        {
        if (p == null)
        throw new ArgumentNullException(nameof(p));
            
            return new PropertyResponseDto
            {
                Id = p.Id,
                ListerId = p.ListerId,
                Title = p.Title,
                Description = p.Description,
                Price = p.Price,
                AreaSqFt = p.AreaSqFt,
                PropertyType = p.PropertyType.ToString(),
                ListingPurpose = p.ListingPurpose.ToString(),
                ListerType = p.ListerType.ToString(),
                Status = p.Status.ToString(),
                CreatedAt = p.CreatedAt,
                Location = new PropertyLocationResponseDto
                {
                    Locality = p.Location.Locality,
                    City = p.Location.City,
                    State = p.Location.State,
                    Latitude = p.Location.Latitude,
                    Longitude = p.Location.Longitude
                },
                ImageUrls = p.PropertyImages?.Select(i => i.ImageUrl).ToList() ?? new(),
                FeatureSummary = p.Features
                    .GroupBy(f => f.FeatureId)
                    .Select(group =>
                    {
                        var first = group.First();
                        var feature = first.Feature;

                        List<string> values;

                        if (feature.DataType == FeatureDataType.MultiSelect)
                        {
                            values = group
                                .Select(g => g.Option?.Value ?? g.Value)
                                .Where(v => !string.IsNullOrWhiteSpace(v))
                                .ToList();
                        }
                        else
                        {
                            var value = feature.DataType == FeatureDataType.Dropdown
                                        ? first.Option?.Value
                                        : first.Value;

                            values = !string.IsNullOrWhiteSpace(value)
                                ? new List<string> { value }
                                : new List<string>();
                        }

                        return new PropertyFeatureResponseDto
                        {
                            FeatureId = feature.Id,
                            FeatureName = feature.Name,
                            Values = values
                        };
                    })
                .OrderBy(f => f.FeatureName)
                .ToList()

            };
        }

        private string SanitizeText(string input)
        {
            return string.Join(" ", input.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries));
        }

    }
}

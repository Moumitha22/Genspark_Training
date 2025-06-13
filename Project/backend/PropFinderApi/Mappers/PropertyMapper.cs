using PropFinderApi.Models;
using PropFinderApi.Models.DTOs;

namespace PropFinderApi.Mappers
{
    public class PropertyMapper
    {
        public Property MapPropertyAddRequestDtoToProperty(PropertyAddRequestDto dto, Guid agentId)
        {
            return new Property
            {
                Id = Guid.NewGuid(),
                AgentId = agentId,
                Title = SanitizeText(dto.Title),
                Description = SanitizeText(dto.Description ?? ""),
                Price = dto.Price,
                Location = SanitizeText(dto.Location),
                PropertyType = dto.PropertyType,
                ListingType = dto.ListingType,
                Bedrooms = dto.Bedrooms ?? 0,
                Bathrooms = dto.Bathrooms ?? 0,
                AreaSqFt = dto.AreaSqFt,
                IsFurnished = dto.IsFurnished ?? false,
                HasBalcony = dto.HasBalcony ?? false,
                HasParking = dto.HasParking ?? false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Status = "Available",
                IsDeleted = false
            };
        }

        public Property UpdatePropertyFromDto(Property existing, PropertyUpdateRequestDto dto)
        {
            if (!string.IsNullOrWhiteSpace(dto.Title))
                existing.Title = SanitizeText(dto.Title);

            if (!string.IsNullOrWhiteSpace(dto.Description))
                existing.Description = SanitizeText(dto.Description);

            if (dto.Price.HasValue)
                existing.Price = dto.Price.Value;

            if (!string.IsNullOrWhiteSpace(dto.Location))
                existing.Location = SanitizeText(dto.Location);

            if (dto.PropertyType.HasValue)
                existing.PropertyType = dto.PropertyType.Value;

            if (dto.ListingType.HasValue)
                existing.ListingType = dto.ListingType.Value;

            if (dto.Bedrooms.HasValue)
                existing.Bedrooms = dto.Bedrooms;

            if (dto.Bathrooms.HasValue)
                existing.Bathrooms = dto.Bathrooms;

            if (dto.AreaSqFt.HasValue)
                existing.AreaSqFt = dto.AreaSqFt.Value;

            if (dto.IsFurnished.HasValue)
                existing.IsFurnished = dto.IsFurnished.Value;

            if (dto.HasBalcony.HasValue)
                existing.HasBalcony = dto.HasBalcony.Value;

            if (dto.HasParking.HasValue)
                existing.HasParking = dto.HasParking.Value;

            existing.UpdatedAt = DateTime.UtcNow;

            return existing;
        }


        public PropertyResponseDto MapPropertyToPropertyResponseDto(Property property)
        {
            return new PropertyResponseDto
            {
                Id = property.Id,
                AgentId = property.AgentId,
                Title = property.Title,
                Description = property.Description,
                Price = property.Price,
                Location = property.Location,
                PropertyType = property.PropertyType.ToString(),
                ListingType = property.ListingType.ToString(),
                Bedrooms = property.Bedrooms,
                Bathrooms = property.Bathrooms,
                IsFurnished = property.IsFurnished,
                HasBalcony = property.HasBalcony,
                HasParking = property.HasParking,
                AreaSqFt = property.AreaSqFt,
                CreatedAt = property.CreatedAt,
                Status = property.Status,
                ImageUrls = property.PropertyImages?
                    .Select(img => img.ImageUrl)
                    .ToList() ?? new()
            };
        }
        private string SanitizeText(string input)
        {
            return string.Join(" ", input.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries));
        }

    }
}

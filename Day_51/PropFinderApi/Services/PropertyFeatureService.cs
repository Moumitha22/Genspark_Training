using PropFinderApi.Interfaces;
using PropFinderApi.Models;
using PropFinderApi.Models.DTOs;

namespace PropFinderApi.Services
{
    public class PropertyFeatureService : IPropertyFeatureService
    {
        private readonly IPropertyFeatureRepository _propertyFeatureRepository;

        public PropertyFeatureService(IPropertyFeatureRepository propertyFeatureRepository)
        {
            _propertyFeatureRepository = propertyFeatureRepository;
        }

        public async Task UpdateFeatureSetAsync(Guid propertyId, List<PropertyFeatureAddRequestDto> dtos)
        {
            var existing = (await _propertyFeatureRepository.GetByPropertyIdAsync(propertyId)).ToList();
            var newFeatures = new List<PropertyFeature>();

            foreach (var dto in dtos)
            {
                var dataType = Normalize(dto.DataType);

                var match = dataType switch
                {
                    "multiselect" => existing.FirstOrDefault(f => f.FeatureId == dto.FeatureId && f.OptionId == dto.OptionId),
                    _ => existing.FirstOrDefault(f => f.FeatureId == dto.FeatureId)
                };

                if (match != null)
                {
                    match.Value = dataType is not ("dropdown" or "multiselect") ? dto.Value : null;
                    match.OptionId = dataType is "dropdown" or "multiselect" ? dto.OptionId : null;
                    match.IsDeleted = false;
                    match.UpdatedAt = DateTime.UtcNow;
                }
                else
                {
                    newFeatures.Add(new PropertyFeature
                    {
                        Id = Guid.NewGuid(),
                        PropertyId = propertyId,
                        FeatureId = dto.FeatureId,
                        Value = dataType is not ("dropdown" or "multiselect") ? dto.Value : null,
                        OptionId = dataType is "dropdown" or "multiselect" ? dto.OptionId : null,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        IsDeleted = false
                    });
                }
            }

            foreach (var existingFeature in existing)
            {
                bool stillExists = dtos.Any(dto =>
                {
                    var dataType = Normalize(dto.DataType);
                    return dto.FeatureId == existingFeature.FeatureId &&
                           (dataType == "multiselect" ? dto.OptionId == existingFeature.OptionId : true);
                });

                if (!stillExists)
                {
                    existingFeature.IsDeleted = true;
                    existingFeature.UpdatedAt = DateTime.UtcNow;
                }
            }

            if (newFeatures.Any())
                await _propertyFeatureRepository.AddRangeAsync(newFeatures);

            await _propertyFeatureRepository.SaveAsync();
        }

        private static string Normalize(string? type) =>
            type?.Trim().ToLowerInvariant() ?? string.Empty;
    }
}

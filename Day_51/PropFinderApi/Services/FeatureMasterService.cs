using PropFinderApi.Exceptions;
using PropFinderApi.Interfaces;
using PropFinderApi.Models;
using PropFinderApi.Models.DTOs;
using PropFinderApi.Models.Enums;

namespace PropFinderApi.Services
{
    public class FeatureMasterService : IFeatureMasterService
    {
        private readonly IFeatureMasterRepository _featureMasterRepository;

        public FeatureMasterService(IFeatureMasterRepository featureMasterRepository)
        {
            _featureMasterRepository = featureMasterRepository;
        }

        public async Task<FeatureFieldDto> CreateFeatureAsync(FeatureMasterAddRequestDto dto)
        {
            var feature = new FeatureMaster
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                DataType = dto.DataType,
                FilterMode = dto.FilterMode,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Options = new List<FeatureOption>(),
                Applicability = new List<FeatureApplicability>()
            };

            if ((dto.DataType == FeatureDataType.Dropdown || dto.DataType == FeatureDataType.MultiSelect)
                && dto.Options != null && dto.Options.Any())
            {
                feature.Options = dto.Options.Select(opt => new FeatureOption
                {
                    Id = Guid.NewGuid(),
                    Value = opt,
                    FeatureId = feature.Id,
                    IsDeleted = false
                }).ToList();
            }

            feature.Applicability = dto.Applicabilities.Select(a => new FeatureApplicability
            {
                Id = Guid.NewGuid(),
                FeatureId = feature.Id,
                AppliesToType = a.AppliesToType,
                AppliesToPurpose = a.AppliesToPurpose,
                IsDeleted = false
            }).ToList();

            await _featureMasterRepository.Add(feature);
            return new FeatureFieldDto
            {
                Id = feature.Id,
                Name = feature.Name,
                DataType = feature.DataType.ToString(),
                FilterMode = feature.FilterMode.ToString(),
                Options = feature.Options?.Select(o => new FeatureOptionDto
                {
                    Id = o.Id,
                    Value = o.Value
                }).ToList() ?? new()
            };
        }

        public async Task<FeatureMaster> GetAsync(Guid featureId)
        {
            return await _featureMasterRepository.Get(featureId);
        }

        public async Task<IEnumerable<FeatureAdminDto>> GetAllFeaturesAsync()
        {
            var features = await _featureMasterRepository.GetAll();

            return features.Select(f => new FeatureAdminDto
            {
                Id = f.Id,
                Name = f.Name,
                DataType = f.DataType.ToString(),
                FilterMode = f.FilterMode.ToString(),

                Options = f.Options?
                    .Where(o => !o.IsDeleted)
                    .Select(o => new FeatureOptionDto
                    {
                        Id = o.Id,
                        Value = o.Value
                    }).ToList() ?? new(),

                Applicability = f.Applicability?
                    .Where(a => !a.IsDeleted)
                    .Select(a => new FeatureApplicabilityDto
                    {
                        AppliesToPurpose = a.AppliesToPurpose,
                        AppliesToType = a.AppliesToType
                    }).ToList() ?? new()
            }).ToList();
        }

        public async Task<IEnumerable<FeatureFieldDto>> GetApplicableFeaturesAsync(PropertyType propertyType, ListingPurpose listingPurpose)
        {
            var applicableFeatures = await _featureMasterRepository.GetApplicableFeaturesAsync(propertyType, listingPurpose);
            return MapToDto(applicableFeatures);
        }

        public async Task<IEnumerable<FeatureFieldDto>> GetApplicableFeaturesByPurposeAsync(ListingPurpose listingPurpose)
        {
            var applicableFeatures = await _featureMasterRepository.GetApplicableFeaturesByPurposeAsync(listingPurpose);
            return MapToDto(applicableFeatures);
        }


        public async Task<bool> SoftDeleteFeatureAsync(Guid featureId)
        {
            var feature = await _featureMasterRepository.Get(featureId);

            if (feature.IsDeleted)
                throw new ConflictException("Feature already deleted");

            feature.IsDeleted = true;
            feature.UpdatedAt = DateTime.UtcNow;

            await _featureMasterRepository.Update(featureId, feature);
            return true;
        }

        private List<FeatureFieldDto> MapToDto(IEnumerable<FeatureMaster> features)
        {
            return features.Select(f => new FeatureFieldDto
            {
                Id = f.Id,
                Name = f.Name,
                DataType = f.DataType.ToString(),
                FilterMode = f.FilterMode.ToString(),

                Options = f.Options?
                    .Where(o => !o.IsDeleted)
                    .Select(o => new FeatureOptionDto
                    {
                        Id = o.Id,
                        Value = o.Value
                    }).ToList() ?? new()
            }).ToList();
        }


        public async Task<FeatureFieldDto> UpdateFeatureAsync(Guid featureId, FeatureMasterAddRequestDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            await _featureMasterRepository.UpdateFeatureWithOptionsAsync(featureId, dto);

            var updated = await _featureMasterRepository.Get(featureId);

            return new FeatureFieldDto
            {
                Id = updated.Id,
                Name = updated.Name,
                DataType = updated.DataType.ToString(),
                FilterMode = updated.FilterMode.ToString(),
                Options = updated.Options?
                    .Where(o => !o.IsDeleted)
                    .Select(o => new FeatureOptionDto
                    {
                        Id = o.Id,
                        Value = o.Value
                    }).ToList() ?? new()
            };

        }


    }
}

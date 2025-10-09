using Microsoft.EntityFrameworkCore;
using PropFinderApi.Contexts;
using PropFinderApi.Exceptions;
using PropFinderApi.Interfaces;
using PropFinderApi.Models;
using PropFinderApi.Models.DTOs;
using PropFinderApi.Models.Enums;

namespace PropFinderApi.Repositories
{
    public class FeatureMasterRepository : Repository<Guid, FeatureMaster>, IFeatureMasterRepository
    {
        public FeatureMasterRepository(PropFinderDbContext context) : base(context) { }

        public override async Task<FeatureMaster> Get(Guid key)
        {
            var feature = await _propFinderDbContext.FeatureMasters
                .Include(f => f.Options)
                .Include(f => f.Applicability)
                .SingleOrDefaultAsync(fm => fm.Id == key);

            return feature ?? throw new NotFoundException($"Feature with ID {key} not found");
        }


        public override async Task<IEnumerable<FeatureMaster>> GetAll()
        {
            return await _propFinderDbContext.FeatureMasters
                .Where(f => !f.IsDeleted)
                .Include(f => f.Options)
                .Include(f => f.Applicability)
                .OrderBy(f => f.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<FeatureMaster>> GetApplicableFeaturesAsync(PropertyType propertyType, ListingPurpose listingPurpose)
        {
            return await _propFinderDbContext.FeatureMasters
                .Include(f => f.Options)
                .Include(f => f.Applicability)
                .Where(f => !f.IsDeleted && f.Applicability.Any(a =>
                    !a.IsDeleted &&
                    a.AppliesToType == propertyType &&
                    a.AppliesToPurpose == listingPurpose))
                .OrderBy(f => f.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<FeatureMaster>> GetApplicableFeaturesByPurposeAsync(ListingPurpose purpose)
        {
            return await _propFinderDbContext.FeatureMasters
                .Include(f => f.Options)
                .Include(f => f.Applicability)
                 .Where(f => !f.IsDeleted && f.Applicability.Any(a =>
                        !a.IsDeleted &&
                        a.AppliesToPurpose == purpose))
                .OrderBy(f => f.Name)
                .ToListAsync();
        }
      
        public async Task UpdateFeatureWithOptionsAsync(Guid featureId, FeatureMasterAddRequestDto dto)
        {
            var feature = await _propFinderDbContext.FeatureMasters
                .Include(f => f.Options)
                .Include(f => f.Applicability)
                .FirstOrDefaultAsync(f => f.Id == featureId);

            if (feature == null || feature.IsDeleted)
                throw new NotFoundException($"Feature with ID {featureId} not found");

            feature.Name = dto.Name;
            feature.DataType = dto.DataType;
            feature.FilterMode = dto.FilterMode;
            feature.UpdatedAt = DateTime.UtcNow;

            var incomingOptions = (dto.Options ?? new())
                .Select(o => o.Trim())
                .Where(o => !string.IsNullOrWhiteSpace(o))
                .Distinct()
                .ToList();

            foreach (var option in feature.Options)
            {
                if (!incomingOptions.Contains(option.Value))
                {
                    option.IsDeleted = true;
                }
            }

            foreach (var val in incomingOptions)
            {
                var existing = feature.Options.FirstOrDefault(o => o.Value == val);
                if (existing != null)
                {
                    if (existing.IsDeleted)
                        existing.IsDeleted = false;
                }
                else
                {
                    await _propFinderDbContext.FeatureOptions.AddAsync(new FeatureOption
                    {
                        Id = Guid.NewGuid(),
                        FeatureId = feature.Id,
                        Value = val,
                        IsDeleted = false
                    });
                }
            }

            var incomingApplicabilities = dto.Applicabilities ?? new();

            foreach (var existing in feature.Applicability)
            {
                bool stillExists = incomingApplicabilities.Any(incoming =>
                    incoming.AppliesToType == existing.AppliesToType &&
                    incoming.AppliesToPurpose == existing.AppliesToPurpose);

                if (!stillExists)
                {
                    existing.IsDeleted = true;
                }
            }

            foreach (var incoming in incomingApplicabilities)
            {
                var existing = feature.Applicability.FirstOrDefault(a =>
                    a.AppliesToType == incoming.AppliesToType &&
                    a.AppliesToPurpose == incoming.AppliesToPurpose);

                if (existing != null)
                {
                    if (existing.IsDeleted)
                        existing.IsDeleted = false;
                }
                else
                {
                    await _propFinderDbContext.FeatureApplicabilities.AddAsync(new FeatureApplicability
                    {
                        Id = Guid.NewGuid(),
                        FeatureId = feature.Id,
                        AppliesToType = incoming.AppliesToType,
                        AppliesToPurpose = incoming.AppliesToPurpose,
                        IsDeleted = false
                    });
                }

            }

            await _propFinderDbContext.SaveChangesAsync();
        }


    }
}

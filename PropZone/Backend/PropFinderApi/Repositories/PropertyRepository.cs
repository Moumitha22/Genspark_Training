using Microsoft.EntityFrameworkCore;
using PropFinderApi.Contexts;
using PropFinderApi.Exceptions;
using PropFinderApi.Interfaces;
using PropFinderApi.Mappers;
using PropFinderApi.Models;
using PropFinderApi.Models.DTOs;
using PropFinderApi.Models.Enums;

namespace PropFinderApi.Repositories
{
    public class PropertyRepository : Repository<Guid, Property>, IPropertyRepository
    {
        public PropertyRepository(PropFinderDbContext context)
            : base(context) { }

        public override async Task<IEnumerable<Property>> GetAll()
        {
            var properties = await _propFinderDbContext
                .Properties.AsNoTracking()
                .AsSplitQuery()
                .Include(p => p.PropertyImages)
                .Include(p => p.Location)
                .Include(p => p.Features.Where(f => !f.IsDeleted && !f.Feature.IsDeleted))
                .ThenInclude(f => f.Feature)
                .Include(p => p.Features.Where(f => !f.IsDeleted && !f.Feature.IsDeleted))
                .ThenInclude(f => f.Option)
                .Where(p => !p.IsDeleted && p.Status != ListingStatus.Deleted)
                .ToListAsync();

            return properties;
        }

        public override async Task<Property> Get(Guid id)
        {
            var property = await _propFinderDbContext
                .Properties.AsSplitQuery()
                .Include(p => p.Lister)
                .Include(p => p.PropertyImages)
                .Include(p => p.Location)
                .Include(p => p.Features)
                .ThenInclude(f => f.Feature)
                .Include(p => p.Features)
                .ThenInclude(f => f.Option)
                .Include(p => p.PropertyDiscountCodes)
                .ThenInclude(pd => pd.DiscountCode)
                .SingleOrDefaultAsync(p => p.Id == id && !p.IsDeleted);

            if (property == null)
                throw new NotFoundException($"Property with ID '{id}' not found");

            return property;
        }

        public async Task<PaginatedResult<Property>> GetByListerIdAsync(
            Guid listerId,
            PaginationModel pagination
        )
        {
            var query = _propFinderDbContext
                .Properties.AsSplitQuery()
                .Include(p => p.PropertyImages)
                .Include(p => p.Location)
                .Include(p => p.Features)
                .ThenInclude(f => f.Feature)
                .Include(p => p.Features)
                .ThenInclude(f => f.Option)
                .Include(p => p.PropertyDiscountCodes)
                .ThenInclude(pd => pd.DiscountCode)
                .ThenInclude(dc => dc.Options)
                .Where(p =>
                    p.ListerId == listerId && !p.IsDeleted && p.Status != ListingStatus.Deleted
                );

            return await ApplyPagination(query, pagination);
        }

        public async Task UpdateStatusAsync(Guid propertyId, string newStatus)
        {
            var property = await _propFinderDbContext.Properties.FindAsync(propertyId);
            if (property == null)
                throw new NotFoundException("Property not found");

            property.UpdatedAt = DateTime.UtcNow;

            _propFinderDbContext.Properties.Update(property);
            await _propFinderDbContext.SaveChangesAsync();
        }

        public async Task<PaginatedResult<Property>> BasicSearchAsync(
            BasicPropertySearchModel query,
            SortModel sort,
            PaginationModel pagination
        )
        {
            var q = _propFinderDbContext
                .Properties.AsNoTracking()
                .AsSplitQuery()
                .Include(p => p.Location)
                .Include(p => p.PropertyImages)
                .Include(p => p.Features)
                .ThenInclude(f => f.Feature)
                .Include(p => p.Features)
                .ThenInclude(f => f.Option)
                .Where(p => !p.IsDeleted && p.Status == ListingStatus.Available);

            if (!string.IsNullOrEmpty(query.Locality))
                q = q.Where(p => p.Location.Locality.ToLower().Contains(query.Locality.ToLower()));

            if (!string.IsNullOrEmpty(query.City))
                q = q.Where(p => p.Location.City.ToLower().Contains(query.City.ToLower()));

            if (query.ListingPurpose.HasValue)
                q = q.Where(p => p.ListingPurpose == query.ListingPurpose);

            if (query.ListerTypes?.Any() == true)
                q = q.Where(p => query.ListerTypes.Contains(p.ListerType));

            if (query.PropertyTypes?.Any() == true)
                q = q.Where(p => query.PropertyTypes.Contains(p.PropertyType));

            if (query.MinPrice.HasValue)
                q = q.Where(p => p.Price >= query.MinPrice.Value);

            if (query.MaxPrice.HasValue)
                q = q.Where(p => p.Price <= query.MaxPrice.Value);

            if (query.MinArea.HasValue)
                q = q.Where(p => p.AreaSqFt >= query.MinArea.Value);

            if (query.MaxArea.HasValue)
                q = q.Where(p => p.AreaSqFt <= query.MaxArea.Value);

            if (query.HasImages == true)
                q = q.Where(p => p.PropertyImages.Any());

            if (query.IsDiscountAvailable == true)
            {
                q = q.Where(p =>
                    p.PropertyDiscountCodes != null
                    && p.PropertyDiscountCodes.Any(dc =>
                        !dc.DiscountCode.IsDeleted
                        && dc.DiscountCode.IsActive
                        && dc.DiscountCode.FromDate <= DateTime.UtcNow
                        && dc.DiscountCode.ToDate >= DateTime.UtcNow
                    )
                );
            }

            if (!string.IsNullOrWhiteSpace(query.Keyword))
            {
                var keyword = query.Keyword.ToLower();
                q = q.Where(p =>
                    p.Title.ToLower().Contains(keyword)
                    || p.Description.ToLower().Contains(keyword)
                    || p.Location.Locality.ToLower().Contains(keyword)
                    || p.Location.City.ToLower().Contains(keyword)
                );
            }

            q = ApplySorting(q, sort);

            return await ApplyPagination(q, pagination);
        }

        public async Task<PaginatedResult<Property>> AdvancedSearchAsync(
            AdvancedPropertySearchModel searchModel,
            SortModel sortModel,
            PaginationModel paginationModel
        )
        {
            var query = _propFinderDbContext
                .Properties.AsNoTracking()
                .AsSplitQuery()
                .Include(p => p.PropertyImages)
                .Include(p => p.Location)
                .Include(p => p.Features)
                .ThenInclude(f => f.Feature)
                .Include(p => p.Features)
                .ThenInclude(f => f.Option)
                .Include(p => p.PropertyDiscountCodes)
                .ThenInclude(pd => pd.DiscountCode)
                .ThenInclude(dc => dc.Options)
                .Where(p => !p.IsDeleted);

            query = ApplyFilters(query, searchModel);
            query = ApplySorting(query, sortModel);

            return await ApplyPagination(query, paginationModel);
        }

        private IQueryable<Property> ApplyFilters(
            IQueryable<Property> query,
            AdvancedPropertySearchModel model
        )
        {
            if (model.ListingPurpose.HasValue)
                query = query.Where(p => p.ListingPurpose == model.ListingPurpose.Value);

            if (model.PropertyTypes?.Any() == true)
                query = query.Where(p => model.PropertyTypes.Contains(p.PropertyType));

            if (model.ListerTypes?.Any() == true)
                query = query.Where(p => model.ListerTypes.Contains(p.ListerType));

            if (model.Statuses?.Any() == true)
                query = query.Where(p => model.Statuses.Contains(p.Status));

            if (model.ListerId.HasValue)
                query = query.Where(p => p.ListerId == model.ListerId.Value);

            if (!string.IsNullOrEmpty(model.Locality))
                query = query.Where(p =>
                    p.Location.Locality.ToLower().Contains(model.Locality.ToLower())
                );

            if (!string.IsNullOrEmpty(model.City))
                query = query.Where(p => p.Location.City.ToLower().Contains(model.City.ToLower()));

            if (!string.IsNullOrEmpty(model.State))
                query = query.Where(p =>
                    p.Location.State.ToLower().Contains(model.State.ToLower())
                );

            if (model.IsDiscountAvailable == true)
            {
                query = query.Where(p =>
                    p.PropertyDiscountCodes != null
                    && p.PropertyDiscountCodes.Any(dc =>
                        !dc.DiscountCode.IsDeleted
                        && dc.DiscountCode.IsActive
                        && dc.DiscountCode.FromDate <= DateTime.UtcNow
                        && dc.DiscountCode.ToDate >= DateTime.UtcNow
                    )
                );
            }

            if (!string.IsNullOrEmpty(model.Keyword))
            {
                var keyword = model.Keyword.ToLower();
                query = query.Where(p =>
                    p.Title.ToLower().Contains(keyword)
                    || p.Description.ToLower().Contains(keyword)
                    || p.Location.Locality.ToLower().Contains(keyword)
                    || p.Location.City.ToLower().Contains(keyword)
                );
            }

            if (model.PriceRange?.Min.HasValue == true)
                query = query.Where(p => p.Price >= model.PriceRange.Min.Value);

            if (model.PriceRange?.Max.HasValue == true)
                query = query.Where(p => p.Price <= model.PriceRange.Max.Value);

            if (model.AreaRange?.Min.HasValue == true)
                query = query.Where(p => p.AreaSqFt >= model.AreaRange.Min.Value);

            if (model.AreaRange?.Max.HasValue == true)
                query = query.Where(p => p.AreaSqFt <= model.AreaRange.Max.Value);

            if (model.PostedBy?.Any() == true)
                query = query.Where(p => model.PostedBy.Contains(p.ListerType));

            if (model.PostedAfter.HasValue)
                query = query.Where(p => p.CreatedAt >= model.PostedAfter.Value);

            if (model.PostedBefore.HasValue)
                query = query.Where(p => p.CreatedAt <= model.PostedBefore.Value);

            if (model.HasImages == true)
                query = query.Where(p => p.PropertyImages.Any());

            //  Dynamic feature filtering
            if (model.FeatureFilters?.Any() == true)
            {
                foreach (var filter in model.FeatureFilters)
                {
                    var values = filter.Values;
                    switch (filter.FilterMode)
                    {
                        case FeatureFilterMode.Boolean:
                            bool boolVal = bool.Parse(values.First());
                            query = query.Where(p =>
                                p.Features.Any(f =>
                                    !f.IsDeleted
                                    && !f.Feature.IsDeleted
                                    && f.FeatureId == filter.FeatureId
                                    && f.Value == boolVal.ToString().ToLower()
                                )
                            );
                            break;

                        case FeatureFilterMode.Exact:
                            query = query.Where(p =>
                                p.Features.Any(f =>
                                    !f.IsDeleted
                                    && !f.Feature.IsDeleted
                                    && f.FeatureId == filter.FeatureId
                                    && (
                                        values.Contains(f.Value!)
                                        || (f.Option != null && values.Contains(f.Option.Value))
                                    )
                                )
                            );
                            break;

                        case FeatureFilterMode.Range:
                            decimal? minVal = null,
                                maxVal = null;

                            if (values.Count > 0 && decimal.TryParse(values[0], out var min))
                                minVal = min;

                            if (values.Count > 1 && decimal.TryParse(values[1], out var max))
                                maxVal = max;

                            if (minVal.HasValue || maxVal.HasValue)
                            {
                                query = query.Where(p =>
                                    p.Features.Any(f =>
                                        !f.IsDeleted
                                        && !f.Feature.IsDeleted
                                        && f.FeatureId == filter.FeatureId
                                        && f.Value != null
                                        && f.Value != ""
                                        && (
                                            (
                                                !minVal.HasValue
                                                || Convert.ToDecimal(f.Value) >= minVal.Value
                                            )
                                            && (
                                                !maxVal.HasValue
                                                || Convert.ToDecimal(f.Value) <= maxVal.Value
                                            )
                                        )
                                    )
                                );
                            }
                            break;
                    }
                }
            }

            return query;
        }

        private IQueryable<Property> ApplySorting(IQueryable<Property> query, SortModel sortModel)
        {
            var sortField = sortModel.SortBy ?? "CreatedAt";
            var ascending = sortModel.Ascending;

            return sortField switch
            {
                "Price" => ascending
                    ? query.OrderBy(p => p.Price)
                    : query.OrderByDescending(p => p.Price),
                "AreaSqFt" => ascending
                    ? query.OrderBy(p => p.AreaSqFt)
                    : query.OrderByDescending(p => p.AreaSqFt),
                "CreatedAt" => ascending
                    ? query.OrderBy(p => p.CreatedAt)
                    : query.OrderByDescending(p => p.CreatedAt),
                _ => query.OrderByDescending(p => p.CreatedAt),
            };
        }

        private async Task<PaginatedResult<Property>> ApplyPagination(
            IQueryable<Property> query,
            PaginationModel pagination
        )
        {
            int totalItems = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(totalItems / (double)pagination.PageSize);

            int currentPage = totalPages == 0 ? 1 : Math.Clamp(pagination.Page, 1, totalPages);

            var items = await query
                .Skip((currentPage - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToListAsync();

            return new PaginatedResult<Property>(
                items,
                totalItems,
                currentPage,
                pagination.PageSize
            );
        }
    }
}

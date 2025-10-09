using Microsoft.EntityFrameworkCore;
using PropFinderApi.Contexts;
using PropFinderApi.Exceptions;
using PropFinderApi.Interfaces;
using PropFinderApi.Models;
using PropFinderApi.Models.DTOs;

namespace PropFinderApi.Repositories
{
    public class DiscountCodeRepository : Repository<Guid, DiscountCode>, IDiscountCodeRepository
    {
        public DiscountCodeRepository(PropFinderDbContext context)
            : base(context) { }

        public async Task<IEnumerable<DiscountCode>> GetActiveCodesAsync(
            ActiveDiscountCodeFilterRequestDto filterRequestDto
        )
        {
            var query = _propFinderDbContext
                .DiscountCodes.Include(dc => dc.Options)
                .Where(dc =>
                    dc.IsActive
                    && dc.FromDate <= DateTime.UtcNow
                    && dc.ToDate >= DateTime.UtcNow
                    && !dc.IsDeleted
                    && dc.Options.Any() // only discount with options shown
                );
            if (filterRequestDto.TypeOfProperty.HasValue)
            {
                query = query.Where(dc =>
                    dc.Options.Any(a => a.TypeOfProperty == filterRequestDto.TypeOfProperty.Value)
                );
            }
            if (filterRequestDto.PurposeOfListing.HasValue)
            {
                query = query.Where(dc =>
                    dc.Options.Any(a =>
                        a.PurposeOfListing == filterRequestDto.PurposeOfListing.Value
                    )
                );
            }

            if (filterRequestDto.Price.HasValue)
            {
                query = query.Where(dc =>
                    dc.Options.Any(opt =>
                        !opt.MinPrice.HasValue
                        || (
                            (filterRequestDto.Price.Value >= opt.MinPrice.Value)
                                && (filterRequestDto.Price.Value <= opt.MaxPrice.Value)
                            || !opt.MaxPrice.HasValue
                        )
                    )
                );
            }
            return await query.OrderBy(dc => dc.FromDate).ToListAsync();
        }

        public override async Task<DiscountCode> Get(Guid key)
        {
            var discountCode = await _propFinderDbContext
                .DiscountCodes.Include(dc => dc.Options)
                .SingleOrDefaultAsync(dc => dc.Id == key);

            return discountCode
                ?? throw new NotFoundException($"Discount code with ID {key} not found.");
        }

        public override async Task<IEnumerable<DiscountCode>> GetAll()
        {
            return await _propFinderDbContext
                .DiscountCodes.Include(dc => dc.Options)
                .OrderBy(dc => dc.FromDate)
                .ToListAsync();
        }

        public async Task<DiscountCode> GetByCode(string code)
        {
            var discountCode = await _propFinderDbContext
                .DiscountCodes.Include(dc => dc.Options)
                .SingleOrDefaultAsync(dc => dc.Code.ToLower() == code.ToLower());
            return discountCode;
        }

        public async Task<PaginatedResult<DiscountCode>> SearchAsync(
            BasicDiscountFilterModel filter,
            SortModel sort,
            PaginationModel pagination
        )
        {
            var query = _propFinderDbContext.DiscountCodes.Include(dc => dc.Options).AsQueryable();

            if (!string.IsNullOrEmpty(filter.Code))
                query = query.Where(dc => dc.Code.ToLower().Contains(filter.Code.ToLower()));
            if (filter.MinDiscountValue.HasValue)
                query = query.Where(dc => dc.DiscountValue >= filter.MinDiscountValue.Value);
            if (filter.MaxDiscountValue.HasValue)
                query = query.Where(dc => dc.DiscountValue <= filter.MaxDiscountValue.Value);
            if (filter.IsPercentage.HasValue)
                query = query.Where(dc => dc.IsPercentage == filter.IsPercentage.Value);
            if (filter.FromDate.HasValue)
            {
                var fromDate = filter.FromDate.Value;
                if (fromDate.Kind != DateTimeKind.Utc)
                    fromDate = DateTime.SpecifyKind(fromDate, DateTimeKind.Utc);

                query = query.Where(dc => dc.FromDate >= fromDate);
            }

            if (filter.ToDate.HasValue)
            {
                var toDate = filter.ToDate.Value;
                if (toDate.Kind != DateTimeKind.Utc)
                    toDate = DateTime.SpecifyKind(toDate, DateTimeKind.Utc);

                query = query.Where(dc => dc.ToDate <= toDate);
            }
            if (filter.IsDeleted.HasValue)
                query = query.Where(dc => dc.IsDeleted == filter.IsDeleted.Value);
            if (filter.IsActive.HasValue)
                query = query.Where(dc => dc.IsActive == filter.IsActive.Value);

            if (filter.Purpose.HasValue)
            {
                query = query.Where(dc =>
                    dc.Options.Any(opt => opt.PurposeOfListing == filter.Purpose.Value)
                );
            }

            if (filter.TypeOfProperty.HasValue)
            {
                query = query.Where(dc =>
                    dc.Options.Any(opt => opt.TypeOfProperty == filter.TypeOfProperty.Value)
                );
            }

            var totalItems = await query.CountAsync();

            query = ApplySorting(query, sort);

            return await ApplyPagination(query, pagination);
        }

        private IQueryable<DiscountCode> ApplySorting(
            IQueryable<DiscountCode> query,
            SortModel sort
        )
        {
            if (sort == null || string.IsNullOrEmpty(sort.SortBy))
                return query;

            var sortField = sort.SortBy ?? "CreatedAt";
            var isAscending = sort.Ascending;

            return sortField switch
            {
                "CreatedAt" => isAscending
                    ? query.OrderBy(dc => dc.CreatedAt)
                    : query.OrderByDescending(dc => dc.CreatedAt),
                "Code" => isAscending
                    ? query.OrderBy(dc => dc.Code)
                    : query.OrderByDescending(dc => dc.Code),
                "DiscountValue" => isAscending
                    ? query.OrderBy(dc => dc.DiscountValue)
                    : query.OrderByDescending(dc => dc.DiscountValue),
                "IsPercentage" => isAscending
                    ? query.OrderBy(dc => dc.IsPercentage)
                    : query.OrderByDescending(dc => dc.IsPercentage),
                "FromDate" => isAscending
                    ? query.OrderBy(dc => dc.FromDate)
                    : query.OrderByDescending(dc => dc.FromDate),
                "ToDate" => isAscending
                    ? query.OrderBy(dc => dc.ToDate)
                    : query.OrderByDescending(dc => dc.ToDate),
                _ => query.OrderBy(dc => dc.CreatedAt),
            };
        }

        public async Task<IEnumerable<DiscountCode>> GetByIds(IEnumerable<Guid> ids)
        {
            return await _propFinderDbContext
                .DiscountCodes.Where(dc => ids.Contains(dc.Id) && !dc.IsDeleted)
                .Include(dc => dc.Options)
                .OrderBy(dc => dc.FromDate)
                .ToListAsync();
        }

        private async Task<PaginatedResult<DiscountCode>> ApplyPagination(
            IQueryable<DiscountCode> query,
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

            return new PaginatedResult<DiscountCode>(
                items,
                totalItems,
                currentPage,
                pagination.PageSize
            );
        }
    }
}

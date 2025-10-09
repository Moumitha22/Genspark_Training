using Microsoft.EntityFrameworkCore;
using PropFinderApi.Contexts;
using PropFinderApi.Interfaces;
using PropFinderApi.Mappers;
using PropFinderApi.Models;
using PropFinderApi.Models.DTOs;
using PropFinderApi.Models.Enums;

namespace PropFinderApi.Services
{
    public class DiscountCodeService : IDiscountCodeService
    {
        private readonly IDiscountCodeRepository _discountCodeRepository;
        private readonly DiscountCodeMapper _discountCodeMapper;
        private readonly PropFinderDbContext _context;

        public DiscountCodeService(
            IDiscountCodeRepository discountCodeRepository,
            PropFinderDbContext context
        )
        {
            _discountCodeRepository = discountCodeRepository;
            _discountCodeMapper = new DiscountCodeMapper();

            _context = context;
        }

        public async Task<DiscountCodeDto> CreateDiscountCodeAsync(
            DiscountCodeAddRequestDto requestDto
        )
        {
            if (
                string.IsNullOrWhiteSpace(requestDto.Code)
                || requestDto.DiscountValue <= 0
                || requestDto.FromDate == default
                || requestDto.ToDate == default
            )
            {
                throw new ArgumentException("Invalid discount code data.");
            }

            if (requestDto.FromDate >= requestDto.ToDate)
            {
                throw new ArgumentException("FromDate must be earlier than ToDate.");
            }

            if (requestDto.ToDate < DateTime.UtcNow)
            {
                throw new ArgumentException("ToDate must be in the future.");
            }

            if (requestDto.DiscountValue < 0)
            {
                throw new ArgumentException("Discount value must be a positive number.");
            }

            if (requestDto.IsPercentage && requestDto.DiscountValue > 100)
            {
                throw new ArgumentException("Percentage discount cannot exceed 100.");
            }

            var existingCode = await _discountCodeRepository.GetByCode(requestDto.Code);
            if (existingCode != null)
            {
                var existingCodeEndDate = existingCode?.ToDate;
                if (existingCodeEndDate.HasValue && existingCodeEndDate.Value > DateTime.UtcNow)
                {
                    throw new ArgumentException(
                        $"Discount code '{requestDto.Code}' is already active until {existingCodeEndDate.Value}."
                    );
                }
            }

            var discountCode = _discountCodeMapper.MapToEntity(requestDto);
            discountCode.IsActive = true;
            var addedDiscountCode = await _discountCodeRepository.Add(discountCode);
            return _discountCodeMapper.MapToDto(addedDiscountCode);
        }

        public async Task<IEnumerable<DiscountCodeDto>> GetActiveDiscountCodesAsync(
            ActiveDiscountCodeFilterRequestDto filterRequestDto
        )
        {
            var entites = await _discountCodeRepository.GetActiveCodesAsync(filterRequestDto);
            return entites.Select(_discountCodeMapper.MapToDto).ToList();
        }

        public async Task<DiscountCodeDto> GetDiscountCodeByIdAsync(Guid id)
        {
            var entity = await _discountCodeRepository.Get(id);
            if (entity == null)
            {
                throw new KeyNotFoundException("Discount code not found.");
            }
            return _discountCodeMapper.MapToDto(entity);
        }

        public async Task<DiscountCodeDto> UpdateDiscountCodeAsync(
            Guid id,
            DiscountCodeUpdateRequestDto request
        )
        {
            var existingCode = await _discountCodeRepository.Get(id);
            if (existingCode == null)
            {
                throw new KeyNotFoundException("Discount code not found.");
            }

            _discountCodeMapper.MapUpdateDto(request, existingCode);

            await _discountCodeRepository.Update(id, existingCode);
            return _discountCodeMapper.MapToDto(existingCode);
        }

        public async Task<PaginatedResult<DiscountCodeDto>> SearchDiscountCodesAsync(
            BasicDiscountFilterModel filterRequest,
            SortModel sortModel,
            PaginationModel paginationModel
        )
        {
            var result = await _discountCodeRepository.SearchAsync(
                filterRequest,
                sortModel,
                paginationModel
            );

            return MapPaginatedResult(result, _discountCodeMapper.MapToDto);
        }

        public async Task<DiscountSimulationResponseDto> SimulateDiscountAsync(
            DiscountSimulationRequest dto
        )
        {
            var discountCodes =
                (dto.DiscountCodeIds != null && dto.DiscountCodeIds.Any())
                    ? await _discountCodeRepository.GetByIds(dto.DiscountCodeIds)
                    : new List<DiscountCode>();

            var originalPrice = dto.Price;
            var discountedPrice = originalPrice;

            if (discountCodes == null || !discountCodes.Any())
            {
                return new DiscountSimulationResponseDto
                {
                    OriginalPrice = originalPrice,
                    DiscountedPrice = originalPrice,
                };
            }

            foreach (var code in discountCodes)
            {
                if (
                    code.IsActive
                    && code.FromDate <= DateTime.UtcNow
                    && code.ToDate >= DateTime.UtcNow
                )
                {
                    if (code.IsPercentage)
                    {
                        discountedPrice -= discountedPrice * (code.DiscountValue / 100);
                    }
                    else
                    {
                        discountedPrice -= code.DiscountValue;
                    }
                }
            }

            return new DiscountSimulationResponseDto
            {
                OriginalPrice = originalPrice,
                DiscountedPrice = Math.Max(discountedPrice, 0),
            };
        }

        // soft delete
        public async Task<bool> UpdateDiscountDeletion(Guid id, bool disable)
        {
            var entity = await _discountCodeRepository.Get(id);
            if (entity == null)
            {
                throw new KeyNotFoundException("Discount code not found");
            }

            entity.IsDeleted = disable;
            var updatedEntity = await _discountCodeRepository.Update(id, entity);
            return updatedEntity != null;
        }

        private PaginatedResult<TDestination> MapPaginatedResult<TSource, TDestination>(
            PaginatedResult<TSource> source,
            Func<TSource, TDestination> mapFunc
        )
        {
            return new PaginatedResult<TDestination>(
                source.Items.Select(mapFunc).ToList(),
                source.Pagination.TotalItems,
                source.Pagination.CurrentPage,
                source.Pagination.PageSize
            );
        }
    }
}

using PropFinderApi.Models;
using PropFinderApi.Models.DTOs;

namespace PropFinderApi.Interfaces
{
    public interface IDiscountCodeService
    {
        Task<DiscountCodeDto> CreateDiscountCodeAsync(DiscountCodeAddRequestDto request);
        Task<IEnumerable<DiscountCodeDto>> GetActiveDiscountCodesAsync(
            ActiveDiscountCodeFilterRequestDto filterRequestDto
        );
        Task<DiscountCodeDto> GetDiscountCodeByIdAsync(Guid id);
        Task<PaginatedResult<DiscountCodeDto>> SearchDiscountCodesAsync(
            BasicDiscountFilterModel filterRequest,
            SortModel sortModel,
            PaginationModel paginationModel
        );
        Task<DiscountSimulationResponseDto> SimulateDiscountAsync(DiscountSimulationRequest dto);
        Task<DiscountCodeDto> UpdateDiscountCodeAsync(
            Guid id,
            DiscountCodeUpdateRequestDto request
        );
        Task<bool> UpdateDiscountDeletion(Guid id, bool disable);
    }
}

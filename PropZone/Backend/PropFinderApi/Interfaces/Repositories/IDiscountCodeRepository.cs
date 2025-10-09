using PropFinderApi.Models;
using PropFinderApi.Models.DTOs;

namespace PropFinderApi.Interfaces
{
    public interface IDiscountCodeRepository : IRepository<Guid, DiscountCode>
    {
        Task<DiscountCode> GetByCode(string code);
        Task<IEnumerable<DiscountCode>> GetActiveCodesAsync(
            ActiveDiscountCodeFilterRequestDto filterRequestDto
        );
        Task<IEnumerable<DiscountCode>> GetByIds(IEnumerable<Guid> ids);

        Task<PaginatedResult<DiscountCode>> SearchAsync(
            BasicDiscountFilterModel filter,
            SortModel sort,
            PaginationModel pagination
        );
    }
}

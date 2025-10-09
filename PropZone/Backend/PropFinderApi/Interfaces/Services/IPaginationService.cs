using PropFinderApi.Models.DTOs;

namespace PropFinderApi.Interfaces
{
    public interface IPaginationService
    {
        (List<T> PaginatedData, PaginationInfoDto Pagination) ApplyPagination<T>(
            IEnumerable<T> source, int page, int pageSize);
    }

}
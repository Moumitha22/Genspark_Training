using PropFinderApi.Interfaces;
using PropFinderApi.Models.DTOs;

namespace PropFinderApi.Services
{
    public class PaginationService : IPaginationService
    {
        public (List<T> PaginatedData, PaginationInfoDto Pagination) ApplyPagination<T>(
            IEnumerable<T> source, int page, int pageSize)
        {
            int totalItems = source.Count();
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            int currentPage = totalPages > 0 ? page : 1;

            if (page > totalPages)
            {
                page = totalPages; // Auto-adjust to last available page
            }

            var paginatedData = source
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var pagination = new PaginationInfoDto
            {
                CurrentPage = currentPage,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = totalPages
            };

            return (paginatedData, pagination);
        }
    }

}
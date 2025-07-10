namespace PropFinderApi.Models.DTOs
{
    public class PaginatedResult<T>
    {
        public IEnumerable<T> Items { get; set; }
        public PaginationInfoDto Pagination { get; set; }

        public PaginatedResult(IEnumerable<T> items, int totalItems, int currentPage, int pageSize)
        {
            Items = items;

            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            Pagination = new PaginationInfoDto
            {
                CurrentPage = currentPage,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = totalPages
            };
        }
    }
}

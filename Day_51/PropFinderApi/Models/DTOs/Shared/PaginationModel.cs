namespace PropFinderApi.Models.DTOs
{
    public class PaginationModel
    {
        public int Page { get; set; } = 1;          // Default to first page
        public int PageSize { get; set; } = 10;     // Default page size
    }
}

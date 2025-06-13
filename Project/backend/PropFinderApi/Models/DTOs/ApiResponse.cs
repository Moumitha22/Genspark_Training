namespace PropFinderApi.Models.DTOs
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public PaginationInfoDto? Pagination { get; set; }
        public Dictionary<string, string[]>? Errors { get; set; }
    }

}
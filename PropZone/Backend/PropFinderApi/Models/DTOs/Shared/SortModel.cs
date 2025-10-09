namespace PropFinderApi.Models.DTOs
{
    public class SortModel
    {
        public string SortBy { get; set; } = "CreatedAt"; 
        public bool Ascending { get; set; } = false; 
    }

}
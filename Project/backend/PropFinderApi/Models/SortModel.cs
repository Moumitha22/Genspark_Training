namespace PropFinderApi.Models
{
    public class SortModel
    {
        public string SortBy { get; set; } = "CreatedAt"; 
        public bool Ascending { get; set; } = false; 
    }

}
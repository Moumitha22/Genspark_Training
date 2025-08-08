namespace OnlineAuctionAPI.Models.DTO
{
    public class BidFilterDto
    {
        public string? Name { get; set; }                
        public decimal? AmountMin { get; set; }
        public decimal? AmountMax { get; set; }
        public DateTime? DateMin { get; set; }  
        public DateTime? DateMax { get; set; }          
        public string? SortBy { get; set; }          
        public string? SortDirection { get; set; } = "asc";
    }
    
}
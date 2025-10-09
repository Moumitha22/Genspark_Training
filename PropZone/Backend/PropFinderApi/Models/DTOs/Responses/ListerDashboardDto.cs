namespace PropFinderApi.Models.DTOs
{
    public class ListerDashboardDto
    {
        public int TotalPropertiesListed { get; set; }
        public int TotalForSale { get; set; }
        public int TotalForRent { get; set; }
        public int TotalSoldOut { get; set; }
        public int TotalRented { get; set; }
        public int TotalAvailable { get; set; }
        public int TotalInquiriesReceived { get; set; }
        public List<ChartItemDto> PropertyTypeChart { get; set; } = new();
        public List<ChartItemDto> PropertyPurposeChart { get; set; } = new();
        public List<ChartItemDto> PropertyStatusChart { get; set; } = new();
    }
}
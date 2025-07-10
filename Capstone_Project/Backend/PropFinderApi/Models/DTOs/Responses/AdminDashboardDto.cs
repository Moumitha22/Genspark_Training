using PropFinderApi.Models.Enums;

namespace PropFinderApi.Models.DTOs
{
    public class AdminDashboardDto
    {
        public int TotalUsers { get; set; }
        public int TotalProperties { get; set; }
        public int TotalInquiries { get; set; }
        public int TotalActiveListers { get; set; }

        public List<ChartItemDto> PropertyTypeChart { get; set; } = new();
        public List<ChartItemDto> PropertyPurposeChart { get; set; } = new();
        public List<ChartItemDto> PropertyStatusChart { get; set; } = new();
    }
}
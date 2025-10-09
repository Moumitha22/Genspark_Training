namespace PropFinderApi.Models.DTOs
{
    public class DiscountCodeAddRequestDto
    {
        public string Code { get; set; } = string.Empty;
        public decimal DiscountValue { get; set; }
        public bool IsPercentage { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public bool IsActive { get; set; } = true;
        public int? MaxListerLimit { get; set; }
        public List<DiscountCodeOptionsDto> Options { get; set; } = new();
    }
}

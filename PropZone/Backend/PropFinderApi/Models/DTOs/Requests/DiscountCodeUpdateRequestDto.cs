namespace PropFinderApi.Models.DTOs
{
    public class DiscountCodeUpdateRequestDto
    {
        public string? Code { get; set; }
        public decimal? DiscountValue { get; set; }
        public bool? IsPercentage { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public bool? IsActive { get; set; }
        public int? MaxListerLimit { get; set; }
        public List<DiscountCodeOptionsDto>? Options { get; set; }
    }
}

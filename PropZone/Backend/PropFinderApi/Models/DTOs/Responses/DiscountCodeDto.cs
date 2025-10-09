namespace PropFinderApi.Models.DTOs
{
    public class DiscountCodeDto
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public decimal DiscountValue { get; set; }
        public bool IsPercentage { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
        public int? MaxListerLimit { get; set; }
        public List<DiscountCodeOptionsDto> Options { get; set; } = new();
    }
}

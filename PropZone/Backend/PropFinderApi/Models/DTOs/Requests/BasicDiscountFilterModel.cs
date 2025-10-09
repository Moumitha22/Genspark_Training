using PropFinderApi.Models.Enums;

namespace PropFinderApi.Models.DTOs
{
    public class BasicDiscountFilterModel
    {
        public string? Code { get; set; }
        public decimal? MinDiscountValue { get; set; }
        public decimal? MaxDiscountValue { get; set; }
        public bool? IsPercentage { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public bool? IsDeleted { get; set; }
        public bool? IsActive { get; set; }
        public ListingPurpose? Purpose { get; set; }
        public PropertyType? TypeOfProperty { get; set; }
    }
}

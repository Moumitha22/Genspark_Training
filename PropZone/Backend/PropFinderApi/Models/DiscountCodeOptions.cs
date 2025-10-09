using PropFinderApi.Models.Enums;

namespace PropFinderApi.Models
{
    public class DiscountCodeOptions
    {
        public Guid Id { get; set; }
        public PropertyType? TypeOfProperty { get; set; }
        public ListingPurpose? PurposeOfListing { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public Guid DiscountCodeId { get; set; }
        public DiscountCode DiscountCode { get; set; }
    }
}

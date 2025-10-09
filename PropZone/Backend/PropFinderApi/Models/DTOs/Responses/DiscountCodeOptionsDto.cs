using PropFinderApi.Models.Enums;

namespace PropFinderApi.Models.DTOs
{
    public class DiscountCodeOptionsDto
    {
        public PropertyType? TypeOfProperty { get; set; }
        public ListingPurpose? PurposeOfListing { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
    }
}

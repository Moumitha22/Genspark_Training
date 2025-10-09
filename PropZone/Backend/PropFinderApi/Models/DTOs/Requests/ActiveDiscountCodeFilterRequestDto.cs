using PropFinderApi.Models;
using PropFinderApi.Models.Enums;

namespace PropFinderApi.Models.DTOs
{
    public class ActiveDiscountCodeFilterRequestDto
    {
        public PropertyType? TypeOfProperty { get; set; }
        public ListingPurpose? PurposeOfListing { get; set; }
        public decimal? Price { get; set; }
    }
}

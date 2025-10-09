using PropFinderApi.Models.Enums;

namespace PropFinderApi.Models.DTOs
{
    public class BasicPropertySearchModel
    {
        public string? Locality { get; set; }
        public string? City { get; set; }
        public ListingPurpose? ListingPurpose { get; set; }
        public List<ListerType>? ListerTypes { get; set; }
        public List<PropertyType>? PropertyTypes { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string? Keyword { get; set; }
        public int? MinArea { get; set; }
        public int? MaxArea { get; set; }
        public bool? HasImages { get; set; }
        public bool? IsDiscountAvailable { get; set; }
    }

}
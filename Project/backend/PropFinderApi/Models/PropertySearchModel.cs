using PropFinderApi.Models.Enums;

namespace PropFinderApi.Models
{
    public class PropertySearchModel
    {
        public Guid? AgentId { get; set; }
        public string? Title { get; set; } 
        public string? Description { get; set; }
        public string? Location { get; set; }
        public GenericRangeModel<int>? PriceRange { get; set; } 
        public GenericRangeModel<int>? BedroomsRange { get; set; } 
        public GenericRangeModel<int>? BathroomsRange { get; set; } 
        public GenericRangeModel<int>? AreaRange { get; set; } 
        public PropertyType? PropertyType { get; set; }
        public ListingType? ListingType { get; set; } 
        public bool? IsFurnished { get; set; } 
        public bool? HasParking { get; set; }
        public bool? HasBalcony { get; set; } 
        public DateTime? PostedAfter { get; set; } 
        public DateTime? PostedBefore { get; set; } 
    }

}
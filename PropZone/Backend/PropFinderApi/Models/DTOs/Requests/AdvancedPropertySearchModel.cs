using Microsoft.AspNetCore.Mvc;
using PropFinderApi.Models.Enums;

namespace PropFinderApi.Models.DTOs
{
    public class AdvancedPropertySearchModel
    {
        // 1. Core Filters
        public ListingPurpose? ListingPurpose { get; set; } // Buy / Rent
        public List<PropertyType>? PropertyTypes { get; set; }
        public List<ListerType>? ListerTypes { get; set; }
        public Guid? ListerId { get; set; }

        // 2. Location & Keyword
        public string? Locality { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Keyword { get; set; }

        // 3. Price & Area
        public GenericRangeModel<decimal>? PriceRange { get; set; }
        public GenericRangeModel<decimal>? AreaRange { get; set; }

        // 4. Posting & Status
        public List<ListerType>? PostedBy { get; set; } // Owner / Agent
        public List<ListingStatus>? Statuses { get; set; } // Available / Sold / Rented
        public DateTime? PostedAfter { get; set; }
        public DateTime? PostedBefore { get; set; }

        // 5. Optional Derived Filters
        public bool? HasImages { get; set; }
        public bool? IsDiscountAvailable { get; set; }

        // 6. Dynamic Feature Filters
        public List<DynamicFeatureFilter>? FeatureFilters { get; set; } = new();
    }

}
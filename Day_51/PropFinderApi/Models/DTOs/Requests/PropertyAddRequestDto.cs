using System.ComponentModel.DataAnnotations;
using PropFinderApi.Models.Enums;

namespace PropFinderApi.Models.DTOs
{
    public class PropertyAddRequestDto
    {
        [Required(ErrorMessage = "Title is required.")]
        [StringLength(150, ErrorMessage = "Title cannot exceed 150 characters.")]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
        public string? Description { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero.")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Lister type is required.")]
        [EnumDataType(typeof(ListerType), ErrorMessage = "Invalid property type. Property type can be Apartment, House, Villa, Plot or Commercial")]
       
        public ListerType ListerType { get; set; }

        [Required(ErrorMessage = "Property type is required.")]
        [EnumDataType(typeof(PropertyType), ErrorMessage = "Invalid lister type. Lister type can eiher be 'Agent' or 'Owner'")]
        public PropertyType PropertyType { get; set; }

        [Required(ErrorMessage = "Property listing type is required.")]
        [EnumDataType(typeof(ListingPurpose), ErrorMessage = "Invalid property listing purpose. Listing purpose can either be 'Sale' or 'Rent'")]
        public ListingPurpose ListingPurpose { get; set; }
        
        public ListingStatus Status { get; set; }

        [Range(0, 100, ErrorMessage = "Bedrooms must be between 0 and 100.")]
        public int? Bedrooms { get; set; }

        [Range(0, 100, ErrorMessage = "Bathrooms must be between 0 and 100.")]
        public int? Bathrooms { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Area (sq ft) must be at least 1 sq ft.")]
        public decimal AreaSqFt { get; set; }

        [Required(ErrorMessage = "Location is required.")]
        public PropertyLocationAddRequestDto Location { get; set; } = new();

        public List<PropertyFeatureAddRequestDto> Features { get; set; } = new();

    }
}

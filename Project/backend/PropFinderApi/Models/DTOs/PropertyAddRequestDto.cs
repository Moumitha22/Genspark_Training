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

        [Required(ErrorMessage = "Location is required.")]
        [StringLength(200, ErrorMessage = "Location cannot exceed 200 characters.")]
        public string Location { get; set; } = string.Empty;

        [Required(ErrorMessage = "Property type is required.")]
        [EnumDataType(typeof(PropertyType), ErrorMessage = "Invalid property type. Property type can be Apartment, House, Villa, Plot or Commercial")]
        public PropertyType PropertyType { get; set; }

        [Required(ErrorMessage = "Property listing type is required.")]
        [EnumDataType(typeof(ListingType), ErrorMessage = "Invalid property listing type. Listing type can either be 'Sale' or 'Rent'")]
        public ListingType ListingType { get; set; }

        [Range(0, 100, ErrorMessage = "Bedrooms must be between 0 and 100.")]
        public int? Bedrooms { get; set; }

        [Range(0, 100, ErrorMessage = "Bathrooms must be between 0 and 100.")]
        public int? Bathrooms { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Area (sq ft) must be at least 1 sq ft.")]
        public decimal AreaSqFt { get; set; }
        public bool? IsFurnished { get; set; } 
        public bool? HasParking { get; set; } 
        public bool? HasBalcony { get; set; }
    }
}

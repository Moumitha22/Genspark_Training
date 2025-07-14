using PropFinderApi.Models.Enums;

namespace PropFinderApi.Models
{
    public class FeatureMaster
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;   
        public FeatureDataType DataType { get; set; }       // Boolean, Text, Number, Dropdown, MultiSelect
        public FeatureFilterMode FilterMode { get; set; }


        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }

        // Navigation
        public ICollection<FeatureOption>? Options { get; set; }    
        public ICollection<FeatureApplicability>? Applicability { get; set; }
    }

    
}
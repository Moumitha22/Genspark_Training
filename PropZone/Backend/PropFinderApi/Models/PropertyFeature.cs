namespace PropFinderApi.Models
{
    public class PropertyFeature
{
    public Guid Id { get; set; }
    public Guid PropertyId { get; set; }
    public Guid FeatureId { get; set; }

    public string? Value { get; set; }      // For text, number, boolean
    public Guid? OptionId { get; set; }     // For dropdown and multiselect
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }

    // Navigation
    public Property Property { get; set; } = null!;
    public FeatureMaster Feature { get; set; } = null!;
    public FeatureOption? Option { get; set; }  // Only for dropdown/multiselect
}

}
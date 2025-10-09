namespace PropFinderApi.Models
{
    public class FeatureOption
    {
        public Guid Id { get; set; }
        public Guid FeatureId { get; set; }
        public string Value { get; set; } = string.Empty;
        public bool IsDeleted { get; set; }

        // Navigation
        public FeatureMaster Feature { get; set; } = null!;
    }
}
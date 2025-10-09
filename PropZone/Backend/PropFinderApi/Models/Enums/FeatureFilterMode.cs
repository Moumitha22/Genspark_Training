namespace PropFinderApi.Models.Enums
{
    public enum FeatureFilterMode
    {
        Exact,         // Match any value exactly (multi-select, like Furnishing)
        Boolean,       // Match true/false
        Range        // For numeric/text ranges (e.g., Floor, Deposit)
    }

}
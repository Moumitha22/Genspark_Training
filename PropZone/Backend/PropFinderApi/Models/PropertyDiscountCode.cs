namespace PropFinderApi.Models
{
    public class PropertyDiscountCode
    {
        public Guid PropertyId { get; set; }
        public Property Property { get; set; } = null!;

        public Guid DiscountCodeId { get; set; }
        public DiscountCode DiscountCode { get; set; } = null!;
    }
}

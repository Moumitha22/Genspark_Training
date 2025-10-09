using System.ComponentModel.DataAnnotations;

namespace PropFinderApi.Models.DTOs
{
    public class DiscountSimulationRequest
    {
        [Required(ErrorMessage = "Discount Code is required.")]
        public List<Guid> DiscountCodeIds { get; set; }
        public decimal Price { get; set; }
    }
}

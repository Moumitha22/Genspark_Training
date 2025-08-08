using System.ComponentModel.DataAnnotations;

namespace OnlineAuctionAPI.Models.DTO
{
    public class BidStatusUpdateRequestDto
    {
        [Required(ErrorMessage = "Bid status is required.")]
        [EnumDataType(typeof(BidStatus), ErrorMessage = "Invalid bid status.")]
        public BidStatus Status { get; set; }
    }
}
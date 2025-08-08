namespace OnlineAuctionAPI.Models.DTO;

public class UserDeleteRequest
{
    public required Guid UserId { get; set; }
    public required string Reason { get; set; }
}
